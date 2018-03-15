// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using WAYWF.Agent.CorDebugApi;

namespace WAYWF.Agent
{
	static class AssemblyExtensions
	{
		public static unsafe string GetName(this ICorDebugAssembly assembly)
		{
			assembly.GetName(0, out var size, null);

			if (size <= 1)
			{
				return string.Empty;
			}

			var buffer = stackalloc char[size];
			assembly.GetName(size, out size, buffer);
			return new string(buffer, 0, size - 1);
		}

		public static bool HasAppDomainInfo(this ICorDebugAssembly assembly)
		{
			var domain = assembly.GetAppDomain();
			return domain.GetID() != 0;
		}
	}
}
