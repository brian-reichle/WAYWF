// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Runtime.InteropServices;
using WAYWF.Agent.CorDebugApi;
using WAYWF.Agent.Win32;

namespace WAYWF.Agent
{
	static class DebugExtensions
	{
		public static ICorDebugProcess DebugActiveProcess(this ICorDebug debug, ProcessHandle handle)
		{
			var hr = debug.DebugActiveProcess(handle.Pid, false, out var process);

			if (hr == HResults.CORDBG_E_DEBUGGER_ALREADY_ATTACHED)
			{
				throw new AttachException(ErrorCodes.AlreadyAttached);
			}
			else if (hr >= 0)
			{
				return process;
			}

			if (hr == HResults.E_ACCESSDENIED && !handle.IsAlive())
			{
				throw AttachException.ProcessTerminatedBeforeAttaching(handle.Pid);
			}

			throw Marshal.GetExceptionForHR(hr);
		}
	}
}
