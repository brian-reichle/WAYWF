// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace WAYWF.Agent.Win32
{
	static partial class NativeMethods
	{
		public const int GW_OWNER = 4;
		public static readonly IntPtr INVALID_HANDLE = (IntPtr)(-1);
	}
}
