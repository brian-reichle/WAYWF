// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using WAYWF.Agent.CorDebugApi;

namespace WAYWF.Agent
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
	}
}
