// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace WAYWF.Api.Win32
{
	static partial class NativeMethods
	{
		public const int MAX_PATH = 260;
		public static readonly IntPtr INVALID_HANDLE = (IntPtr)(-1);
	}
}
