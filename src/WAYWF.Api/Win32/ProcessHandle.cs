// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace WAYWF.Api.Win32
{
	sealed class ProcessHandle : SafeHandle
	{
		public ProcessHandle()
			: base(IntPtr.Zero, true)
		{
		}

		public override bool IsInvalid => handle == IntPtr.Zero || handle == NativeMethods.INVALID_HANDLE;
		protected override bool ReleaseHandle() => NativeMethods.CloseHandle(handle);
	}
}
