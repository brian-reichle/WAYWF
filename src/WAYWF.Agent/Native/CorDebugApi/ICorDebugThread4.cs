// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("1A1F204B-1C66-4637-823F-3EE6C744A69C")]
	interface ICorDebugThread4
	{
		// HRESULT HasUnhandledException();
		[PreserveSig]
		int HasUnhandledException();

		// HRESULT GetBlockingObjects(
		//     [out] ICorDebugBlockingObjectEnum **ppBlockingObjectEnum
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugBlockingObjectEnum GetBlockingObjects();

		// HRESULT GetCurrentCustomDebuggerNotification(
		//     [out] ICorDebugValue ** ppNotificationObject
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugValue GetCurrentCustomDebuggerNotification();
	}
}
