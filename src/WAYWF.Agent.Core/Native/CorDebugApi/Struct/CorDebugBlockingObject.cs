// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Runtime.InteropServices;

namespace WAYWF.Agent.Core.CorDebugApi
{
	[StructLayout(LayoutKind.Sequential)]
	struct CorDebugBlockingObject
	{
		// ICorDebugValue* pBlockingObject;
		[MarshalAs(UnmanagedType.Interface)]
		public ICorDebugValue pBlockingObject;

		// DWORD dwTimeout;
		public int dwTimeout;

		// CorDebugBlockingReason blockingReason;
		public CorDebugBlockingReason blockingReason;
	}
}
