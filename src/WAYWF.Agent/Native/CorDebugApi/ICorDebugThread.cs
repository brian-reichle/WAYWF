// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("938C6D66-7FB6-4F69-B389-425B8987329B")]
	interface ICorDebugThread
	{
		// HRESULT GetProcess(
		//     [out] ICorDebugProcess **ppProcess
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugProcess GetProcess();

		// HRESULT GetID(
		//     [out] DWORD *pdwThreadId
		// );
		int GetID();

		// HRESULT GetHandle(
		//     [out] HTHREAD *phThreadHandle
		// );
		void GetHandle_();

		// HRESULT GetAppDomain(
		//     [out] ICorDebugAppDomain **ppAppDomain
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugAppDomain GetAppDomain();

		// HRESULT SetDebugState(
		//     [in] CorDebugThreadState state
		// );
		void SetDebugState(
			CorDebugThreadState state);

		// HRESULT GetDebugState(
		//     [out] CorDebugThreadState *pState
		// );
		CorDebugThreadState GetDebugState();

		// HRESULT GetUserState(
		//     [out] CorDebugUserState *pState
		// );
		CorDebugUserState GetUserState();

		// HRESULT GetCurrentException(
		//     [out] ICorDebugValue **ppExceptionObject
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugValue GetCurrentException();

		// HRESULT ClearCurrentException();
		void ClearCurrentException();

		// HRESULT CreateStepper(
		//     [out] ICorDebugStepper **ppStepper
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugStepper CreateStepper();

		// HRESULT EnumerateChains(
		//     [out] ICorDebugChainEnum **ppChains
		// );
		[PreserveSig]
		int EnumerateChains(
			[MarshalAs(UnmanagedType.Interface)] out ICorDebugChainEnum ppChains);

		// HRESULT GetActiveChain(
		//     [out] ICorDebugChain **ppChain
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugChain GetActiveChain();

		// HRESULT GetActiveFrame(
		//     [out] ICorDebugFrame **ppFrame
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugFrame GetActiveFrame();

		// HRESULT GetRegisterSet(
		//     [out] ICorDebugRegisterSet **ppRegisters
		// );
		void GetRegisterSet_();

		// HRESULT CreateEval(
		//     [out] ICorDebugEval **ppEval
		// );
		void CreateEval_();

		// HRESULT GetObject(
		//     [out] ICorDebugValue **ppObject
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugValue GetObject();
	}
}
