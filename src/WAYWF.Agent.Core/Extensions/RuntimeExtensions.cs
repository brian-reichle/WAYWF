// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using WAYWF.Agent.Core.CLRHostApi;
using WAYWF.Agent.Core.CorDebugApi;

namespace WAYWF.Agent.Core
{
	static class RuntimeExtensions
	{
		public static unsafe string GetVersionString(this ICLRRuntimeInfo runtime)
		{
			var size = 0;
			var hr = runtime.GetVersionString(null, ref size);

			if (hr < 0 && hr != HResults.E_BUFFER_TOO_SMALL)
			{
				Marshal.ThrowExceptionForHR(hr);
			}
			else if (size == 0)
			{
				return null;
			}

			var buffer = stackalloc char[size];

			hr = runtime.GetVersionString(buffer, ref size);

			if (hr < 0)
			{
				Marshal.ThrowExceptionForHR(hr);
			}

			return new string(buffer, 0, size);
		}

		public static bool IsSupportedVersion(this ICLRRuntimeInfo runtime)
		{
			var version = runtime.GetVersionString();
			return VersionsMatch(version, "v4.0") || VersionsMatch(version, "v2.0");
		}

		public static ICorDebug GetCorDebug(this ICLRRuntimeInfo runtime)
		{
			return (ICorDebug)runtime.GetInterface(CLSID.CLSID_CLRDebuggingLegacy, typeof(ICorDebug).GUID);
		}

		static bool VersionsMatch(string actual, string expected)
		{
			return actual.StartsWith(expected, StringComparison.Ordinal) &&
				(actual.Length == expected.Length || actual[expected.Length] == '.');
		}
	}
}
