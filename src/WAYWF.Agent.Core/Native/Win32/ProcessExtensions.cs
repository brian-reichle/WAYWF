// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace WAYWF.Agent.Core.Win32
{
	static class ProcessExtensions
	{
		const int MAX_PATH = 0x00000104;
		const int TOKEN_QUERY = 0x0008;

		public static string QueryFullProcessImageName(this ProcessHandle handle)
		{
			var buffer = new char[MAX_PATH];
			var size = buffer.Length;

			if (!NativeMethods.QueryFullProcessImageName(handle, 0, buffer, ref size))
			{
				var hr = Marshal.GetHRForLastWin32Error();

				if (hr == HResults.ERROR_GEN_FAILURE)
				{
					// Can fail occasionally if the target process crashed.
					return null;
				}

				throw Marshal.GetExceptionForHR(hr);
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

		public static unsafe void GetUser(this ProcessHandle handle, out string username, out string domainname)
		{
			using (var token = handle.OpenProcessToken())
			using (var buffer = token.GetTokenInformation(TOKEN_INFORMATION_CLASS.TokenUser))
			{
				var user = buffer.Read<TOKEN_USER>(0);
				LookupAccountSid(user.User.Sid, out username, out domainname);
			}
		}

		public static bool IsAlive(this ProcessHandle handle)
		{
			if (!NativeMethods.GetExitCodeProcess(handle, out var exitCode))
			{
				var error = Marshal.GetLastWin32Error();

				if (error == Win32ErrorCodes.STILL_ACTIVE)
				{
					return true;
				}

				throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
			}

			return false;
		}

		public static string GetModuleBaseName(this ProcessHandle handle, IntPtr baseAddress)
		{
			var buffer = new char[261];
			var size = NativeMethods.GetModuleBaseName(handle, baseAddress, buffer, buffer.Length);

			if (size == 0)
			{
				throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
			}

			return new string(buffer, 0, size);
		}

		static TokenHandle OpenProcessToken(this ProcessHandle handle)
		{
			TokenHandle token;

			try
			{
				if (!NativeMethods.OpenProcessToken(handle, TOKEN_QUERY, out token))
				{
					throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
				}
			}
			catch
			{
				handle.Dispose();
				throw;
			}

			return token;
		}

		static UnmanagedBuffer GetTokenInformation(this TokenHandle token, TOKEN_INFORMATION_CLASS tokenInformationClass)
		{
			if (!NativeMethods.GetTokenInformation(token, tokenInformationClass, IntPtr.Zero, 0, out var size))
			{
				if (Marshal.GetLastWin32Error() != Win32ErrorCodes.ERROR_INSUFFICIENT_BUFFER)
				{
					throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
				}
			}

			var buffer = new UnmanagedBuffer(size);

			try
			{
				if (!NativeMethods.GetTokenInformation(token, tokenInformationClass, buffer.DangerousGetHandle(), size, out size))
				{
					throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
				}
			}
			catch
			{
				buffer.Dispose();
				throw;
			}

			return buffer;
		}

		static void LookupAccountSid(IntPtr sid, out string username, out string domainname)
		{
			var name = new StringBuilder(256);
			var domain = new StringBuilder(256);
			var nameLength = name.Capacity;
			var domainLength = domain.Capacity;

			if (!NativeMethods.LookupAccountSid(
				null,
				sid,
				name,
				ref nameLength,
				domain,
				ref domainLength,
				out var use))
			{
				throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
			}

			name.Length = nameLength;
			domain.Length = domainLength;

			username = name.ToString();
			domainname = domain.ToString();
		}
	}
}
