// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Immutable;
using System.Diagnostics;
using WAYWF.Agent.Core.CorDebugApi;
using WAYWF.Agent.Core.Win32;
using WAYWF.Agent.Data;

namespace WAYWF.Agent.Core
{
	sealed class RuntimeProcessBuilder
	{
		public RuntimeProcessBuilder(ManagedCallback callback, ProcessHandle handle, ICorDebugProcess process, CaptureOptions options)
		{
			_cache = new MetaDataCache();
			_objects = new RuntimeValueFactory(_cache, new RuntimeNativeInterfaceFactory(handle, process));
			_options = options;
			_sourceCache = new SourceProvider();
			_callback = callback;
			_handle = handle;
			_process = process;
			_tickDuration = 1d / Stopwatch.Frequency;
		}

		public void ImportFromHandle()
		{
			_native = GetNative(_handle);
		}

		public void ImportStandardProcessDetails()
		{
			_clrVersion = _process.GetVersion();
			_threads = GetThreads();
			_appDomains = GetAppDomains();
		}

		public void WalkHeap(ICorDebugProcess process)
		{
			if (process is ICorDebugProcess5 process5)
			{
				_pendingTasks = new PendingTaskFactory(_cache, _objects, _sourceCache).ExtractPendingTasks(process5);
			}
		}

		public RuntimeProcess GetProcess()
		{
			return new RuntimeProcess(
				_options,
				_native,
				_clrVersion,
				_appDomains,
				_threads,
				_sourceCache.GetAllDocuments(),
				_objects.Values.ToImmutableArray(),
				_pendingTasks);
		}

		public void MarkStartTime()
		{
			_start = Stopwatch.GetTimestamp();
		}

		static RuntimeNative GetNative(ProcessHandle handle)
		{
			handle.GetUser(out var username, out var domainname);

			return new RuntimeNative(
				handle.Pid,
				handle.QueryFullProcessImageName(),
				new RuntimeUser(username, domainname),
				RuntimeWindowLoader.Load(handle.Pid));
		}

		ImmutableArray<RuntimeAppDomain> GetAppDomains()
		{
			var appDomainEnum = _process.EnumerateAppDomains();
			var appDomains = ImmutableArray.CreateBuilder<RuntimeAppDomain>(appDomainEnum.GetCount());
			appDomains.Count = appDomains.Capacity;

			for (var i = 0; i < appDomains.Count; i++)
			{
				appDomainEnum.Next(1, out var appDomain);
				appDomains[i] = GetAppDomain(appDomain);
			}

			return appDomains.MoveToImmutable();
		}

		RuntimeAppDomain GetAppDomain(ICorDebugAppDomain appDomain)
		{
			var assemblyEnum = appDomain.EnumerateAssemblies();
			var allModules = ImmutableArray.CreateBuilder<MetaModule>();

			while (assemblyEnum.Next(1, out var assembly))
			{
				var modules = assembly.EnumerateModules();

				while (modules.Next(1, out var module))
				{
					allModules.Add(_cache.GetModule(module));
				}
			}

			return new RuntimeAppDomain(
				appDomain.GetID(),
				appDomain.GetName(),
				allModules.ToImmutable());
		}

		ImmutableArray<RuntimeThread> GetThreads()
		{
			var threadEnum = _process.EnumerateThreads();
			var threads = ImmutableArray.CreateBuilder<RuntimeThread>(threadEnum.GetCount());
			threads.Count = threads.Capacity;

			for (var i = 0; i < threads.Count; i++)
			{
				threadEnum.Next(1, out var thread);
				threads[i] = GetThread(thread);
			}

			return threads.MoveToImmutable();
		}

		RuntimeThread GetThread(ICorDebugThread thread)
		{
			var chainEnum = thread.EnumerateChains();

			if (chainEnum == null)
			{
				return new RuntimeThread(
					thread.GetID(),
					Translate(thread.GetUserState()),
					ImmutableArray<RuntimeFrameChain>.Empty,
					ImmutableArray<RuntimeBlockingObject>.Empty);
			}
			else
			{
				var chains = GetChains(chainEnum);

				var blockingObjects = thread is ICorDebugThread4 thread4
					? GetBlockingObjects(thread4.GetBlockingObjects())
					: ImmutableArray<RuntimeBlockingObject>.Empty;

				return new RuntimeThread(
					thread.GetID(),
					Translate(thread.GetUserState()),
					chains,
					blockingObjects);
			}
		}

		static RuntimeThreadStates Translate(CorDebugUserState state)
		{
			var result = RuntimeThreadStates.None;

			if ((state & CorDebugUserState.USER_BACKGROUND) != 0)
			{
				result |= RuntimeThreadStates.Background;
			}

			if ((state & CorDebugUserState.USER_THREADPOOL) != 0)
			{
				result |= RuntimeThreadStates.ThreadPool;
			}

			if ((state & CorDebugUserState.USER_UNSTARTED) != 0)
			{
				result |= RuntimeThreadStates.NotStarted;
			}
			else if ((state & CorDebugUserState.USER_STOPPED) != 0)
			{
				result |= RuntimeThreadStates.Stopped;
			}
			else if ((state & CorDebugUserState.USER_SUSPENDED) != 0)
			{
				result |= RuntimeThreadStates.Suspended;
			}

			if ((state & CorDebugUserState.USER_SUSPEND_REQUESTED) != 0)
			{
				result |= RuntimeThreadStates.Suspending;
			}

			if ((state & CorDebugUserState.USER_STOP_REQUESTED) != 0)
			{
				result |= RuntimeThreadStates.Stopping;
			}

			if ((state & CorDebugUserState.USER_WAIT_SLEEP_JOIN) != 0)
			{
				result |= RuntimeThreadStates.WaitSleepJoin;
			}

			return result;
		}

		ImmutableArray<RuntimeFrameChain> GetChains(ICorDebugChainEnum chains)
		{
			var result = ImmutableArray.CreateBuilder<RuntimeFrameChain>(chains.GetCount());
			result.Count = result.Capacity;

			for (var i = 0; i < result.Count; i++)
			{
				chains.Next(1, out var chain);
				result[i] = GetChain(chain);
			}

			return result.MoveToImmutable();
		}

		RuntimeFrameChain GetChain(ICorDebugChain chain)
		{
			var tmp = chain.GetReason();
			RuntimeFrameChainReason reason;

			if ((tmp & CorDebugChainReason.CHAIN_CLASS_INIT) != 0)
			{
				reason = RuntimeFrameChainReason.ClassConstructor;
			}
			else if ((tmp & CorDebugChainReason.CHAIN_EXCEPTION_FILTER) != 0)
			{
				reason = RuntimeFrameChainReason.ExceptionFilter;
			}
			else if ((tmp & CorDebugChainReason.CHAIN_SECURITY) != 0)
			{
				reason = RuntimeFrameChainReason.SecurityEvaluation;
			}
			else
			{
				reason = RuntimeFrameChainReason.Unknown;
			}

			return new RuntimeFrameChain(reason, GetFrames(chain.EnumerateFrames()));
		}

		ImmutableArray<RuntimeFrame> GetFrames(ICorDebugFrameEnum frames)
		{
			var result = ImmutableArray.CreateBuilder<RuntimeFrame>(frames.GetCount());
			result.Count = result.Capacity;

			for (var i = 0; i < result.Count; i++)
			{
				frames.Next(1, out var frame);
				result[i] = GetFrame(frame);
			}

			return result.MoveToImmutable();
		}

		RuntimeFrame GetFrame(ICorDebugFrame frame)
		{
			if (frame is ICorDebugILFrame ilFrame)
			{
				return GetILFrame(ilFrame);
			}
			else
			{
				return GetInternalFrame((ICorDebugInternalFrame)frame);
			}
		}

		RuntimeFrame GetILFrame(ICorDebugILFrame frame)
		{
			var function = frame.GetFunction();
			var typeArgs = frame is ICorDebugILFrame2 frame2
				? _cache.GetTypes(frame2.EnumerateTypeParameters())
				: ImmutableArray<MetaTypeBase>.Empty;

			var metaFunc = _cache.GetMethod(function);
			var signature = metaFunc.Signature;

			ImmutableArray<RuntimeValue> paramValues;
			RuntimeValue @this;

			if (signature.Parameters.Length > 0)
			{
				int offset;

				if (signature.CallingConventions.HasImplicitThis())
				{
					offset = 1;
					var value = frame.GetArgument(0);
					@this = value == null ? null : _objects.GetValue(value);
				}
				else
				{
					offset = 0;
					@this = null;
				}

				paramValues = ExtractArguments(frame, signature.Parameters, offset);
			}
			else
			{
				paramValues = ImmutableArray<RuntimeValue>.Empty;
				@this = null;
			}

			var localValues = metaFunc.Locals.Length > 0
				? ExtractLocals(frame, metaFunc.Locals)
				: ImmutableArray<RuntimeValue>.Empty;

			var ilMapping = frame.GetIP(out var ilOffset);

			SourceRef source;
			ImmutableArray<string> localNames;

			if (SouldCaptureSource(ilMapping))
			{
				var module = function.GetModule();
				var token = function.GetToken();

				source = _sourceCache.GetSourceRef(module, token, ilOffset);
				localNames = _sourceCache.GetLocalNames(module, token, ilOffset, metaFunc.Locals.Length);
			}
			else
			{
				source = null;
				localNames = ImmutableArray<string>.Empty;
			}

			var ilFrame = new RuntimeILFrame(
				metaFunc,
				ilOffset,
				TranslateMapping(ilMapping),
				source,
				@this,
				typeArgs,
				paramValues,
				localValues,
				localNames);

			Register(frame, ilFrame);

			return ilFrame;
		}

		static RuntimeILMapping TranslateMapping(CorDebugMappingResult ilMapping)
		{
			return ilMapping switch
			{
				CorDebugMappingResult.MAPPING_EXACT => RuntimeILMapping.Exact,
				CorDebugMappingResult.MAPPING_APPROXIMATE => RuntimeILMapping.Approximate,
				CorDebugMappingResult.MAPPING_PROLOG => RuntimeILMapping.Prolog,
				CorDebugMappingResult.MAPPING_EPILOG => RuntimeILMapping.Epilog,
				_ => RuntimeILMapping.Unmapped,
			};
		}

		ImmutableArray<RuntimeValue> ExtractArguments(ICorDebugILFrame frame, ImmutableArray<MetaVariable> paramDefs, int offset)
		{
			var paramValues = ImmutableArray.CreateBuilder<RuntimeValue>(paramDefs.Length);
			paramValues.Count = paramDefs.Length;

			for (var i = 0; i < paramValues.Count; i++)
			{
				var value = frame.GetArgument(i + offset);

				if (value != null)
				{
					paramValues[i] = _objects.GetValue(value);
				}
			}

			return paramValues.MoveToImmutable();
		}

		ImmutableArray<RuntimeValue> ExtractLocals(ICorDebugILFrame frame, ImmutableArray<MetaVariable> localDefs)
		{
			var localValues = ImmutableArray.CreateBuilder<RuntimeValue>(localDefs.Length);
			localValues.Count = localDefs.Length;

			for (var i = 0; i < localValues.Count; i++)
			{
				var value = frame.GetLocalVariable(i);

				if (value != null)
				{
					localValues[i] = _objects.GetValue(value);
				}
			}

			return localValues.MoveToImmutable();
		}

		static bool SouldCaptureSource(CorDebugMappingResult ilMapping)
		{
			switch (ilMapping)
			{
				case CorDebugMappingResult.MAPPING_APPROXIMATE:
				case CorDebugMappingResult.MAPPING_EPILOG:
				case CorDebugMappingResult.MAPPING_EXACT:
				case CorDebugMappingResult.MAPPING_PROLOG:
					return true;

				default:
					return false;
			}
		}

		static RuntimeFrame GetInternalFrame(ICorDebugInternalFrame frame)
		{
			return new RuntimeInternalFrame((frame.GetFrameType()) switch
			{
				CorDebugInternalFrameType.STUBFRAME_APPDOMAIN_TRANSITION => RuntimeInternalFrameKind.AppDomainTransition,
				CorDebugInternalFrameType.STUBFRAME_INTERNALCALL => RuntimeInternalFrameKind.InternalCall,
				CorDebugInternalFrameType.STUBFRAME_LIGHTWEIGHT_FUNCTION => RuntimeInternalFrameKind.LightWeightFunction,
				CorDebugInternalFrameType.STUBFRAME_M2U => RuntimeInternalFrameKind.ManagedToUnmanaged,
				CorDebugInternalFrameType.STUBFRAME_U2M => RuntimeInternalFrameKind.UnmanagedToManaged,
				_ => RuntimeInternalFrameKind.Unknown,
			});
		}

		ImmutableArray<RuntimeBlockingObject> GetBlockingObjects(ICorDebugBlockingObjectEnum blockingObjects)
		{
			var result = ImmutableArray.CreateBuilder<RuntimeBlockingObject>(blockingObjects.GetCount());
			result.Count = result.Capacity;

			for (var i = 0; i < result.Count; i++)
			{
				blockingObjects.Next(1, out var tmp);
				result[i] = GetBlockingObject(tmp);
			}

			return result.MoveToImmutable();
		}

		RuntimeBlockingObject GetBlockingObject(CorDebugBlockingObject obj)
		{
			var value = obj.pBlockingObject;

			((ICorDebugHeapValue3)value).GetThreadOwningMonitorLock(out var thread, out var _);

			var reason = obj.blockingReason switch
			{
				CorDebugBlockingReason.BLOCKING_MONITOR_CRITICAL_SECTION => RuntimeBlockingReason.Enter,
				CorDebugBlockingReason.BLOCKING_MONITOR_EVENT => RuntimeBlockingReason.Wait,
				_ => RuntimeBlockingReason.Unknown,
			};

			return new RuntimeBlockingObject(
				_objects.GetValue(value),
				thread == null ? 0 : thread.GetID(),
				obj.dwTimeout,
				reason);
		}

		void Register(ICorDebugILFrame icdframe, RuntimeILFrame rtFrame)
		{
			var stepper = icdframe.CreateStepper();
			stepper.StepOut();

			_callback.RegisterStepAction(
				stepper,
				delegate
				{
					var end = Stopwatch.GetTimestamp();
					rtFrame.Duration = (end - _start) * _tickDuration;
				});
		}

		RuntimeNative _native;
		Version _clrVersion;
		ImmutableArray<RuntimeAppDomain> _appDomains;
		ImmutableArray<RuntimeThread> _threads;
		ImmutableArray<PendingStateMachineTask> _pendingTasks = ImmutableArray<PendingStateMachineTask>.Empty;
		long _start;
		readonly CaptureOptions _options;
		readonly ICorDebugProcess _process;
		readonly ProcessHandle _handle;
		readonly double _tickDuration;
		readonly MetaDataCache _cache;
		readonly SourceProvider _sourceCache;
		readonly RuntimeValueFactory _objects;
		readonly ManagedCallback _callback;
	}
}
