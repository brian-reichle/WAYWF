// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("A69ACAD8-2374-46e9-9FF8-B1F14120D296")]
	interface ICorDebugHeapValue3
	{
		// HRESULT GetThreadOwningMonitorLock(
		//     [out] ICorDebugThread **ppThread,
		//     [out] DWORD *pAcquisitionCount
		// );
		void GetThreadOwningMonitorLock(
			[MarshalAs(UnmanagedType.Interface)] out ICorDebugThread ppThread,
			out int pAcquisitionCount);

		// HRESULT GetMonitorEventWaitList(
		//     [out] ICorDebugThreadEnum **ppThreadEnum
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugThreadEnum GetMonitorEventWaitList();
	}
}
