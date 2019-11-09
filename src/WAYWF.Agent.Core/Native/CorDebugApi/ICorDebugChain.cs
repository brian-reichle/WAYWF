// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.Core.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("CC7BCAEE-8A68-11D2-983C-0000F808342D")]
	interface ICorDebugChain
	{
		// HRESULT GetThread(
		//     [out] ICorDebugThread    **ppThread
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugThread GetThread();

		// HRESULT GetStackRange(
		//     [out] CORDB_ADDRESS      *pStart,
		//     [out] CORDB_ADDRESS      *pEnd
		// );
		void GetStackRange(
			out CORDB_ADDRESS pStart,
			out CORDB_ADDRESS pEnd);

		// HRESULT GetContext(
		//     [out] ICorDebugContext   **ppContext
		// );
		void GetContext_();

		// HRESULT GetCaller(
		//     [out] ICorDebugChain      **ppChain
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugChain GetCaller();

		// HRESULT GetCallee(
		//     [out] ICorDebugChain     **ppChain
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugChain GetCallee();

		// HRESULT GetPrevious(
		//     [out] ICorDebugChain     **ppChain
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugChain GetPrevious();

		// HRESULT GetNext(
		//     [out] ICorDebugChain     **ppChain
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugChain GetNext();

		// HRESULT IsManaged(
		//     [out] BOOL               *pManaged
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		bool IsManaged();

		// HRESULT EnumerateFrames(
		//     [out] ICorDebugFrameEnum **ppFrames
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugFrameEnum EnumerateFrames();

		// HRESULT GetActiveFrame(
		//     [out] ICorDebugFrame   **ppFrame
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugFrame GetActiveFrame();

		// HRESULT GetRegisterSet(
		//     [out] ICorDebugRegisterSet **ppRegisters
		// );
		void GetRegisterSet_();

		// HRESULT GetReason(
		//     [out] CorDebugChainReason *pReason
		// );
		CorDebugChainReason GetReason();
	}
}
