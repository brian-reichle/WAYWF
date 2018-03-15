// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace WAYWF.Agent.Win32
{
	[StructLayout(LayoutKind.Sequential)]
	struct SID_AND_ATTRIBUTES
	{
		// PSID  Sid;
		public IntPtr Sid;

		// DWORD Attributes;
		public int Attributes;
	}
}
