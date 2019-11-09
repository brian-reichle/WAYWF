// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Runtime.InteropServices;
using WAYWF.Agent.Core.CorDebugApi;

namespace WAYWF.Agent.Core
{
	static class AppDomainExtensions
	{
		public static unsafe string GetName(this ICorDebugAppDomain appDomain)
		{
			appDomain.GetName(0, out var size, null);

			if (size <= 1)
			{
				return string.Empty;
			}

			var buffer = stackalloc char[size];
			appDomain.GetName(size, out size, buffer);
			return new string(buffer, 0, size - 1);
		}

		public static ICorDebugModule GetModuleFromMetaDataInterface(this ICorDebugAppDomain appDomain, object metadata)
		{
			var hr = appDomain.GetModuleFromMetaDataInterface(metadata, out var module);

			if (hr < 0)
			{
				if (hr == HResults.E_INVALIDARG)
				{
					// We get E_INVALIDARG if metadata belongs to an assembly not loaded into this appDomain.
					// We return null here so the caller can look in the next AppDomain
					return null;
				}

				Marshal.ThrowExceptionForHR(hr);
			}

			return module;
		}
	}
}
