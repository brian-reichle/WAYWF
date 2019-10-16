// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using WAYWF.Agent.CorDebugApi;
using WAYWF.Agent.MetaCache;
using WAYWF.Agent.PendingTasks;
using WAYWF.Agent.Source;
using WAYWF.Agent.Win32;

namespace WAYWF.Agent.Data
{
	sealed class RuntimeProcessBuilder
	{
		public RuntimeProcessBuilder(ManagedCallback callback, ProcessHandle handle, ICorDebugProcess process)
		{
			_cache = new MetaDataCache();
			_objects = new RuntimeValueFactory(_cache, new RuntimeNativeInterfaceFactory(handle, process));
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
				_native,
				_clrVersion,
				_appDomains,
				_threads,
				_sourceCache.GetAllDocuments(),
				_objects.Values.ToArray(),
				_pendingTasks ?? Array.Empty<PendingStateMachineTask>());
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

		RuntimeAppDomain[] GetAppDomains()
		{
			var appDomainEnum = _process.EnumerateAppDomains();
			var appDomains = new RuntimeAppDomain[appDomainEnum.GetCount()];

			for (var i = 0; i < appDomains.Length; i++)
			{
				appDomainEnum.Next(1, out var appDomain);
				appDomains[i] = GetAppDomain(appDomain);
			}
			return appDomains;
		}

		RuntimeAppDomain GetAppDomain(ICorDebugAppDomain appDomain)
		{
			var assemblyEnum = appDomain.EnumerateAssemblies();
			var assemblies = new MetaAssembly[assemblyEnum.GetCount()];

			for (var i = 0; i < assemblies.Length; i++)
			{
				assemblyEnum.Next(1, out var assembly);
				assemblies[i] = _cache.GetAssembly(assembly);

				var modules = assembly.EnumerateModules();

				while (modules.Next(1, out var module))
				{
					_cache.GetModule(module);
				}
			}

			return new RuntimeAppDomain(
				appDomain.GetID(),
				appDomain.GetName(),
				assemblies);
		}

		RuntimeThread[] GetThreads()
		{
			var threadEnum = _process.EnumerateThreads();
			var threads = new RuntimeThread[threadEnum.GetCount()];

			for (var i = 0; i < threads.Length; i++)
			{
				threadEnum.Next(1, out var thread);
				threads[i] = GetThread(thread);
			}

			return threads;
		}

		RuntimeThread GetThread(ICorDebugThread thread)
		{
			var chainEnum = thread.EnumerateChains();

			if (chainEnum == null)
			{
				return new RuntimeThread(thread.GetID(), thread.GetUserState(), null, null);
			}
			else
			{
				var chains = GetChains(chainEnum);
				var blockingObjects = thread is ICorDebugThread4 thread4 ? GetBlockingObjects(thread4.GetBlockingObjects()) : null;

				return new RuntimeThread(thread.GetID(), thread.GetUserState(), chains, blockingObjects);
			}
		}

		RuntimeFrameChain[] GetChains(ICorDebugChainEnum chains)
		{
			var result = new RuntimeFrameChain[chains.GetCount()];

			for (var i = 0; i < result.Length; i++)
			{
				chains.Next(1, out var chain);
				result[i] = GetChain(chain);
			}

			return result;
		}

		RuntimeFrameChain GetChain(ICorDebugChain chain)
		{
			return new RuntimeFrameChain(chain.GetReason(), GetFrames(chain.EnumerateFrames()));
		}

		RuntimeFrame[] GetFrames(ICorDebugFrameEnum frames)
		{
			var result = new RuntimeFrame[frames.GetCount()];

			for (var i = 0; i < result.Length; i++)
			{
				frames.Next(1, out var frame);
				result[i] = GetFrame(frame);
			}

			return result;
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
			var typeArgs = frame is ICorDebugILFrame2 frame2 ? _cache.GetTypes(frame2.EnumerateTypeParameters()) : null;
			var metaFunc = _cache.GetMethod(function);
			var signature = metaFunc.Signature;

			RuntimeValue[] paramValues;
			RuntimeValue @this;

			if (signature.Parameters.Count > 0)
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
				paramValues = null;
				@this = null;
			}

			var localValues = metaFunc.Locals.Count > 0 ? ExtractLocals(frame, metaFunc.Locals) : null;
			var ilMapping = frame.GetIP(out var ilOffset);

			SourceRef source;
			string[] localNames;

			if (SouldCaptureSource(ilMapping))
			{
				var module = function.GetModule();
				var token = function.GetToken();

				source = _sourceCache.GetSourceRef(module, token, ilOffset);
				localNames = _sourceCache.GetLocalNames(module, token, ilOffset, metaFunc.Locals.Count);
			}
			else
			{
				source = null;
				localNames = null;
			}

			var ilFrame = new RuntimeILFrame(
				metaFunc,
				ilOffset,
				ilMapping,
				source,
				@this,
				typeArgs,
				paramValues,
				localValues,
				localNames);

			Register(frame, ilFrame);

			return ilFrame;
		}

		RuntimeValue[] ExtractArguments(ICorDebugILFrame frame, ReadOnlyCollection<MetaVariable> paramDefs, int offset)
		{
			var paramValues = new RuntimeValue[paramDefs.Count];

			for (var i = 0; i < paramValues.Length; i++)
			{
				var value = frame.GetArgument(i + offset);

				if (value != null)
				{
					paramValues[i] = _objects.GetValue(value);
				}
			}

			return paramValues;
		}

		RuntimeValue[] ExtractLocals(ICorDebugILFrame frame, ReadOnlyCollection<MetaVariable> localDefs)
		{
			var localValues = new RuntimeValue[localDefs.Count];

			for (var i = 0; i < localValues.Length; i++)
			{
				var value = frame.GetLocalVariable(i);

				if (value != null)
				{
					localValues[i] = _objects.GetValue(value);
				}
			}

			return localValues;
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
			return new RuntimeInternalFrame(frame.GetFrameType());
		}

		RuntimeBlockingObject[] GetBlockingObjects(ICorDebugBlockingObjectEnum blockingObjects)
		{
			var result = new RuntimeBlockingObject[blockingObjects.GetCount()];

			for (var i = 0; i < result.Length; i++)
			{
				blockingObjects.Next(1, out var tmp);
				result[i] = GetBlockingObject(tmp);
			}

			return result;
		}

		RuntimeBlockingObject GetBlockingObject(CorDebugBlockingObject obj)
		{
			var value = obj.pBlockingObject;

			((ICorDebugHeapValue3)value).GetThreadOwningMonitorLock(out var thread, out var acquisitionCount);

			return new RuntimeBlockingObject(
				_objects.GetValue(value),
				thread == null ? 0 : thread.GetID(),
				obj.dwTimeout,
				obj.blockingReason);
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
		RuntimeAppDomain[] _appDomains;
		RuntimeThread[] _threads;
		PendingStateMachineTask[] _pendingTasks;
		long _start;
		readonly ICorDebugProcess _process;
		readonly ProcessHandle _handle;
		readonly double _tickDuration;
		readonly MetaDataCache _cache;
		readonly SourceProvider _sourceCache;
		readonly RuntimeValueFactory _objects;
		readonly ManagedCallback _callback;
	}
}
