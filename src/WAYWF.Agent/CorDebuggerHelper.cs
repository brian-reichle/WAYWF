// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using WAYWF.Agent.CLRHostApi;
using WAYWF.Agent.CorDebugApi;
using WAYWF.Agent.Win32;

namespace WAYWF.Agent
{
	static class CorDebuggerHelper
	{
		public static ProcessHandle OpenProcess(int pid)
		{
			const AccessOptions access =
				AccessOptions.PROCESS_VM_READ |
				AccessOptions.PROCESS_QUERY_INFORMATION |
				AccessOptions.PROCESS_DUP_HANDLE |
				AccessOptions.SYNCHRONIZE;

			var handle = NativeMethods.OpenProcess(access, false, pid);

			if (handle.IsInvalid)
			{
				var err = Marshal.GetLastWin32Error();
				var innerException = new Win32Exception(err);

				if (err == Win32ErrorCodes.ERROR_ACCESS_DENIED)
				{
					throw new AttachException(
						ErrorCodes.ProcessAccessDenied,
						"Unable to open process " + pid + ", access denied. Try running as administrator.",
						innerException);
				}
				else if (err == Win32ErrorCodes.ERROR_INVALID_PARAMETER)
				{
					throw new AttachException(
						ErrorCodes.NoProcess,
						"Unable to open process " + pid + ", process not found.",
						innerException);
				}
				else
				{
					throw new AttachException(
						ErrorCodes.UnknownError,
						"Unable to open process " + pid + " (" + err + ").",
						innerException);
				}
			}

			handle.Pid = pid;
			return handle;
		}

		public static ICorDebug CreateDebuggingInterfaceForProcess(int pid, ProcessHandle handle)
		{
			if (Environment.Is64BitOperatingSystem &&
				IsWow64Process(handle) == Environment.Is64BitProcess)
			{
				var message = Environment.Is64BitProcess
					? "Cannot attach to a 32-bit process, try using the 32-bit version."
					: "Cannot attach to a 64-bit process, try using the 64-bit version.";

				throw new AttachException(ErrorCodes.BitnessMismatch, message);
			}

			var host = CLRCreateMetaHost();
			var hr = host.EnumerateLoadedRuntimes(handle, out var runtimes);

			if (hr == HResults.ERROR_PARTIAL_COPY && !handle.IsAlive())
			{
				throw AttachException.ProcessTerminatedBeforeAttaching(pid);
			}

			var runtime = GetFirstSupportedRuntime(runtimes);
			return runtime.GetCorDebug();
		}

		static bool IsWow64Process(ProcessHandle ph)
		{
			if (!NativeMethods.IsWow64Process(ph, out var result))
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}

			return result;
		}

		static ICLRMetaHost CLRCreateMetaHost()
		{
			var hr = NativeMethods.CLRCreateInstance(
				CLSID.CLSID_CLRMetaHost,
				typeof(ICLRMetaHost).GUID,
				out var result);

			if (hr < 0)
			{
				throw Marshal.GetExceptionForHR(hr);
			}

			return (ICLRMetaHost)result;
		}

		static ICLRRuntimeInfo GetFirstSupportedRuntime(IEnumUnknown runtimes)
		{
			if (runtimes.Next(1, out var tmp))
			{
				do
				{
					var runtime = (ICLRRuntimeInfo)tmp;

					if (runtime.IsSupportedVersion())
					{
						return runtime;
					}
				}
				while (runtimes.Next(1, out tmp));

				throw new AttachException(ErrorCodes.UnsupportedCLR);
			}
			else
			{
				throw new AttachException(ErrorCodes.NoCLRLoaded);
			}
		}
	}
}
