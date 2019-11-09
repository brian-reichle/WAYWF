// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Runtime.InteropServices;

namespace WAYWF.Agent.Core.CorDebugApi
{
	[StructLayout(LayoutKind.Sequential)]
	struct COR_VERSION
	{
		// DWORD dwMajor;
		public int dwMajor;

		// DWORD dwMinor;
		public int dwMinor;

		// DWORD dwBuild;
		public int dwBuild;

		// DWORD dwSubBuild;
		public int dwSubBuild;
	}
}
