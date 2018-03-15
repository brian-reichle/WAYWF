// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.ComponentModel;

namespace WAYWF.Api.Win32
{
	static class SafeWin32
	{
		public static ProcessHandle GetProcessById(int pid)
		{
			var process = NativeMethods.OpenProcess(ProcessAccessOptions.PROCESS_QUERY_LIMITED_INFORMATION, false, pid);

			if (process == null || process.IsInvalid)
			{
				throw new Win32Exception();
			}

			return process;
		}

		public static bool IsWow64Process(this ProcessHandle process)
		{
			if (!NativeMethods.IsWow64Process(process, out var result))
			{
				throw new Win32Exception();
			}

			return result;
		}

		public static string QueryFullProcessImageName(this ProcessHandle process)
		{
			var buffer = new char[NativeMethods.MAX_PATH];
			var size = buffer.Length;

			if (!NativeMethods.QueryFullProcessImageName(process, 0, buffer, ref size))
			{
				throw new Win32Exception();
			}

			if (size == 0)
			{
				return string.Empty;
			}
			else
			{
				return new string(buffer, 0, size);
			}
		}
	}
}
