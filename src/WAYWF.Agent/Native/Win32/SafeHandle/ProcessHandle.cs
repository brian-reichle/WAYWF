// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using static WAYWF.Agent.Win32.NativeMethods;

namespace WAYWF.Agent.Win32
{
	sealed class ProcessHandle : SafeHandle
	{
		ProcessHandle()
			: base(INVALID_HANDLE, true)
		{
		}

		public int Pid { get; set; }

		public override bool IsInvalid => handle == INVALID_HANDLE || handle == IntPtr.Zero;

		[PrePrepareMethod]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected override bool ReleaseHandle() => CloseHandle(handle);
	}
}
