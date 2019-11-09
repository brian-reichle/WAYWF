// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.Core.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("250E5EEA-DB5C-4C76-B6F3-8C46F12E3203")]
	interface ICorDebugManagedCallback2
	{
		// HRESULT FunctionRemapOpportunity(
		//     [in] ICorDebugAppDomain   *pAppDomain,
		//     [in] ICorDebugThread      *pThread,
		//     [in] ICorDebugFunction    *pOldFunction,
		//     [in] ICorDebugFunction    *pNewFunction,
		//     [in] ULONG32               oldILOffset
		// );
		void FunctionRemapOpportunity(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugAppDomain pAppDomain,
			[MarshalAs(UnmanagedType.Interface)] ICorDebugThread pThread,
			IntPtr pOldFunction,
			IntPtr pNewFunction,
			int oldILOffset);

		// HRESULT CreateConnection(
		//     [in] ICorDebugProcess     *pProcess,
		//     [in] CONNID                dwConnectionId,
		//     [in] WCHAR                *pConnName
		// );
		void CreateConnection(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugProcess pProcess,
			int dwConnectionId,
			[MarshalAs(UnmanagedType.LPWStr)] string pConnName);

		// HRESULT ChangeConnection(
		//     [in] ICorDebugProcess     *pProcess,
		//     [in] CONNID                dwConnectionId
		// );
		void ChangeConnection(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugProcess pProcess,
			int dwConnectionId);

		// HRESULT DestroyConnection(
		//     [in] ICorDebugProcess     *pProcess,
		//     [in] CONNID                dwConnectionId
		// );
		void DestroyConnection(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugProcess pProcess,
			int dwConnectionId);

		// HRESULT Exception(
		//     [in] ICorDebugAppDomain            *pAppDomain,
		//     [in] ICorDebugThread               *pThread,
		//     [in] ICorDebugFrame                *pFrame,
		//     [in] ULONG32                       nOffset,
		//     [in] CorDebugExceptionCallbackType dwEventType,
		//     [in] DWORD                         dwFlags
		// );
		void Exception(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugAppDomain pAppDomain,
			[MarshalAs(UnmanagedType.Interface)] ICorDebugThread pThread,
			IntPtr pFrame,
			int nOffset,
			int dwEventType,
			int dwFlags);

		// HRESULT ExceptionUnwind(
		//     [in] ICorDebugAppDomain                  *pAppDomain,
		//     [in] ICorDebugThread                     *pThread,
		//     [in] CorDebugExceptionUnwindCallbackType  dwEventType,
		//     [in] DWORD                                dwFlags
		// );
		void ExceptionUnwind(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugAppDomain pAppDomain,
			[MarshalAs(UnmanagedType.Interface)] ICorDebugThread pThread,
			int dwEventType,
			int dwFlags);

		// HRESULT FunctionRemapComplete(
		//     [in] ICorDebugAppDomain   *pAppDomain,
		//     [in] ICorDebugThread      *pThread,
		//     [in] ICorDebugFunction    *pFunction
		// );
		void FunctionRemapComplete(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugAppDomain pAppDomain,
			[MarshalAs(UnmanagedType.Interface)] ICorDebugThread pThread,
			IntPtr pFunction);

		// HRESULT MDANotification(
		//     [in] ICorDebugController  *pController,
		//     [in] ICorDebugThread      *pThread,
		//     [in] ICorDebugMDA         *pMDA
		// );
		void MDANotification(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugController pController,
			[MarshalAs(UnmanagedType.Interface)] ICorDebugThread pThread,
			IntPtr pMDA);
	}
}
