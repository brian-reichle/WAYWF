// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WAYWF.UI.Win32
{
	sealed class SafeHGlobal : SafeHandle
	{
		public SafeHGlobal()
			: base(IntPtr.Zero, true)
		{
		}

		public override bool IsInvalid => handle == IntPtr.Zero;
		protected override bool ReleaseHandle() => NativeMethods.GlobalFree(handle) != IntPtr.Zero;

		public void TransferOwnershipTo(out IntPtr hMem)
		{
			RuntimeHelpers.PrepareConstrainedRegions();
			try { }
			finally
			{
				hMem = handle;
				SetHandleAsInvalid();
			}
		}

		public void TransferOwnershipAsStreamTo(out IntPtr stream)
		{
			RuntimeHelpers.PrepareConstrainedRegions();
			try { }
			finally
			{
				var hr = NativeMethods.CreateStreamOnHGlobal(handle, true, out stream);
				SetHandleAsInvalid();

				if (hr < 0)
				{
					Marshal.ThrowExceptionForHR(hr);
				}
			}
		}
	}
}
