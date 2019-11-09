// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Runtime.InteropServices;

namespace WAYWF.Agent.Core.CorDebugApi
{
	[StructLayout(LayoutKind.Sequential)]
	struct COR_HEAPINFO
	{
		// BOOL areGCStructuresValid;
		[MarshalAs(UnmanagedType.Bool)]
		public bool areGCStructuresValid;

		// DWORD pointerSize;
		public int pointerSize;

		// DWORD numHeaps;
		public int numHeaps;

		// BOOL concurrent;
		[MarshalAs(UnmanagedType.Bool)]
		public bool concurrent;

		// CorDebugGCType gcType;
		public int gcType;
	}
}
