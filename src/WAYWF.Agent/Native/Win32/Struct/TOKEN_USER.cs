// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Runtime.InteropServices;

namespace WAYWF.Agent.Win32
{
	[StructLayout(LayoutKind.Sequential)]
	struct TOKEN_USER
	{
		public SID_AND_ATTRIBUTES User;
	}
}
