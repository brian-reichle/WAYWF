// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace WAYWF.Agent.Core.Win32
{
	sealed class TokenHandle : SafeHandle
	{
		TokenHandle()
			: base(IntPtr.Zero, true)
		{
		}

		public override bool IsInvalid => handle == IntPtr.Zero;

		[PrePrepareMethod]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected override bool ReleaseHandle() => NativeMethods.CloseHandle(handle);
	}
}
