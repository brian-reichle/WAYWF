// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using WAYWF.Agent.Core.CorDebugApi;

namespace WAYWF.Agent.Core
{
	sealed class ManagedCallback : ICorDebugManagedCallback, ICorDebugManagedCallback2
	{
		public ManagedCallback()
		{
			_loadedEvent = new TaskCompletionSource<object>();
			_terminationEvent = new TaskCompletionSource<object>();
		}

		public void AwaitAttachComplete()
		{
			Task.WaitAny(_loadedEvent.Task, _terminationEvent.Task);
		}

		public void WaitTimeOrTermination(TimeSpan duration)
		{
			_terminationEvent.Task.Wait((int)duration.TotalMilliseconds);
		}

		public void RegisterStepAction(ICorDebugStepper stepper, Action<ICorDebugStepper> action)
		{
			_stepperLookup.Add(stepper, action);
		}

		public void FlushSteppers()
		{
			foreach (var stepper in _stepperLookup.Keys)
			{
				stepper.Deactivate();
			}

			_stepperLookup.Clear();
		}

		#region ICorDebugManagedCallback Members

		public void Breakpoint(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, IntPtr pBreakpoint)
		{
			Marshal.Release(pBreakpoint);
			EndMessage(pAppDomain.GetProcess());
		}

		public void StepComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugStepper pStepper, int reason)
		{
			try
			{
				Publish(pStepper);
			}
			finally
			{
				EndMessage(pAppDomain.GetProcess());
			}
		}

		public void Break(ICorDebugAppDomain pAppDomain, ICorDebugThread thread)
		{
			EndMessage(pAppDomain.GetProcess());
		}

		public void Exception(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, bool unhandled)
		{
			EndMessage(pAppDomain.GetProcess());
		}

		public void EvalComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, IntPtr pEval)
		{
			Marshal.Release(pEval);
			EndMessage(pAppDomain.GetProcess());
		}

		public void EvalException(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, IntPtr pEval)
		{
			Marshal.Release(pEval);
			EndMessage(pAppDomain.GetProcess());
		}

		public void CreateProcess(ICorDebugProcess pProcess)
		{
			EndMessage(pProcess);
		}

		public void ExitProcess(ICorDebugProcess pProcess)
		{
			_terminationEvent.TrySetResult(null);
		}

		public void CreateThread(ICorDebugAppDomain pAppDomain, ICorDebugThread thread)
		{
			_threadStarted = true;
			EndMessage(pAppDomain.GetProcess());
		}

		public void ExitThread(ICorDebugAppDomain pAppDomain, ICorDebugThread thread)
		{
			EndMessage(pAppDomain.GetProcess());
		}

		public void LoadModule(ICorDebugAppDomain pAppDomain, ICorDebugModule pModule)
		{
			EndMessage(pAppDomain.GetProcess());
		}

		public void UnloadModule(ICorDebugAppDomain pAppDomain, ICorDebugModule pModule)
		{
			EndMessage(pAppDomain.GetProcess());
		}

		public void LoadClass(ICorDebugAppDomain pAppDomain, IntPtr c)
		{
			Marshal.Release(c);
			EndMessage(pAppDomain.GetProcess());
		}

		public void UnloadClass(ICorDebugAppDomain pAppDomain, IntPtr c)
		{
			Marshal.Release(c);
			EndMessage(pAppDomain.GetProcess());
		}

		public void DebuggerError(ICorDebugProcess pProcess, int errorHR, int errorCode)
		{
			EndMessage(pProcess);
		}

		public void LogMessage(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, int lLevel, string pLogSwitchName, string pMessage)
		{
			EndMessage(pAppDomain.GetProcess());
		}

		public void LogSwitch(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, int lLevel, int ulReason, string pLogSwitchName, string pParentName)
		{
			EndMessage(pAppDomain.GetProcess());
		}

		public void CreateAppDomain(ICorDebugProcess pProcess, ICorDebugAppDomain pAppDomain)
		{
			pAppDomain.Attach();
			EndMessage(pProcess);
		}

		public void ExitAppDomain(ICorDebugProcess pProcess, ICorDebugAppDomain pAppDomain)
		{
			EndMessage(pProcess);
		}

		public void LoadAssembly(ICorDebugAppDomain pAppDomain, ICorDebugAssembly pAssembly)
		{
			EndMessage(pAppDomain.GetProcess());
		}

		public void UnloadAssembly(ICorDebugAppDomain pAppDomain, ICorDebugAssembly pAssembly)
		{
			EndMessage(pAppDomain.GetProcess());
		}

		public void ControlCTrap(ICorDebugProcess pProcess)
		{
			EndMessage(pProcess);
		}

		public void NameChange(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread)
		{
			EndMessage(pAppDomain.GetProcess());
		}

		public void UpdateModuleSymbols(ICorDebugAppDomain pAppDomain, ICorDebugModule pMmodule, IStream pSymbolStream)
		{
			EndMessage(pAppDomain.GetProcess());
		}

		public void EditAndContinueRemap(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, IntPtr pFunction, bool fAccurate)
		{
			Marshal.Release(pFunction);
			EndMessage(pAppDomain.GetProcess());
		}

		public void BreakpointSetError(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, IntPtr pBreakpoint, int dwError)
		{
			Marshal.Release(pBreakpoint);
			EndMessage(pAppDomain.GetProcess());
		}

		#endregion

		#region ICorDebugManagedCallback2 Members

		public void FunctionRemapOpportunity(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, IntPtr pOldFunction, IntPtr pNewFunction, int oldILOffset)
		{
			Marshal.Release(pOldFunction);
			Marshal.Release(pNewFunction);
			EndMessage(pAppDomain.GetProcess());
		}

		public void CreateConnection(ICorDebugProcess pProcess, int dwConnectionId, string pConnName)
		{
			EndMessage(pProcess);
		}

		public void ChangeConnection(ICorDebugProcess pProcess, int dwConnectionId)
		{
			EndMessage(pProcess);
		}

		public void DestroyConnection(ICorDebugProcess pProcess, int dwConnectionId)
		{
			EndMessage(pProcess);
		}

		public void Exception(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, IntPtr pFrame, int nOffset, int dwEventType, int dwFlags)
		{
			Marshal.Release(pFrame);
			EndMessage(pAppDomain.GetProcess());
		}

		public void ExceptionUnwind(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, int dwEventType, int dwFlags)
		{
			EndMessage(pAppDomain.GetProcess());
		}

		public void FunctionRemapComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, IntPtr pFunction)
		{
			Marshal.Release(pFunction);
			EndMessage(pAppDomain.GetProcess());
		}

		public void MDANotification(ICorDebugController pController, ICorDebugThread pThread, IntPtr pMDA)
		{
			Marshal.Release(pMDA);

			if (pController is ICorDebugAppDomain domain)
			{
				EndMessage(domain.GetProcess());
			}
			else
			{
				EndMessage((ICorDebugProcess)pController);
			}
		}

		#endregion

		void EndMessage(ICorDebugProcess process)
		{
			if (_threadStarted && !process.HasQueuedCallbacks())
			{
				if (_loadedEvent.TrySetResult(null))
				{
					return;
				}
			}

			process.Continue();
		}

		void Publish(ICorDebugStepper stepper)
		{
			if (_stepperLookup.TryGetValue(stepper, out var action))
			{
				action(stepper);
			}
		}

		bool _threadStarted;
		TaskCompletionSource<object> _loadedEvent;
		TaskCompletionSource<object> _terminationEvent;

		readonly Dictionary<ICorDebugStepper, Action<ICorDebugStepper>> _stepperLookup = new Dictionary<ICorDebugStepper, Action<ICorDebugStepper>>();
	}
}
