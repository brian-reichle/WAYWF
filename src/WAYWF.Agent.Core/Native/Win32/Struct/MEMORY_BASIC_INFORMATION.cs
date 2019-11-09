// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace WAYWF.Agent.Core.Win32
{
	[StructLayout(LayoutKind.Sequential)]
	struct MEMORY_BASIC_INFORMATION
	{
		// PVOID BaseAddress;
		public IntPtr BaseAddress;

		// PVOID AllocationBase;
		public IntPtr AllocationBase;

		// DWORD AllocationProtect;
		public int AllocationProtect;

		// SIZE_T RegionSize;
		public IntPtr RegionSize;

		// DWORD State;
		public int State;

		// DWORD Protect;
		public int Protect;

		// DWORD Type;
		public MEM_FLAGS Type;
	}
}
