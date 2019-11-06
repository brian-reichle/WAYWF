// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;
using WAYWF.Agent.Data;

namespace WAYWF.Agent.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("03E26311-4F76-11D3-88C6-006097945418")]
	interface ICorDebugILFrame : ICorDebugFrame
	{
		// HRESULT GetChain(
		//     [out] ICorDebugChain     **ppChain
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		new ICorDebugChain GetChain();

		// HRESULT GetCode(
		//     [out] ICorDebugCode      **ppCode
		// );
		new void GetCode_();

		// HRESULT GetFunction(
		//     [out] ICorDebugFunction  **ppFunction
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		new ICorDebugFunction GetFunction();

		// HRESULT GetFunctionToken(
		//     [out] mdMethodDef        *pToken
		// );
		new MetaDataToken GetFunctionToken();

		// HRESULT GetStackRange(
		//     [out] CORDB_ADDRESS      *pStart,
		//     [out] CORDB_ADDRESS      *pEnd
		// );
		new void GetStackRange(
			out CORDB_ADDRESS pStart,
			out CORDB_ADDRESS pEnd);

		// HRESULT GetCaller(
		//     [out] ICorDebugFrame     **ppFrame
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		new ICorDebugFrame GetCaller();

		// HRESULT GetCallee(
		//     [out] ICorDebugFrame     **ppFrame
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		new ICorDebugFrame GetCallee();

		// HRESULT CreateStepper(
		//     [out] ICorDebugStepper   **ppStepper
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		new ICorDebugStepper CreateStepper();

		// HRESULT GetIP(
		//     [out] ULONG32               *pnOffset,
		//     [out] CorDebugMappingResult *pMappingResult
		// );
		CorDebugMappingResult GetIP(
			out int pnOffset);

		// HRESULT SetIP(
		//     [in] ULONG32 nOffset
		// );
		void SetIP(
			int nOffset);

		// HRESULT EnumerateLocalVariables(
		//     [out] ICorDebugValueEnum    **ppValueEnum
		// );
		void EnumerateLocalVariables_();

		// HRESULT GetLocalVariable(
		//     [in] DWORD                  dwIndex,
		//     [out] ICorDebugValue      **ppValue
		// );
		[PreserveSig]
		int GetLocalVariable(
			int dwIndex,
			[MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

		// HRESULT EnumerateArguments(
		//     [out] ICorDebugValueEnum    **ppValueEnum
		// );
		void EnumerateArguments_();

		// HRESULT GetArgument(
		//     [in] DWORD                  dwIndex,
		//     [out] ICorDebugValue      **ppValue
		// );
		[PreserveSig]
		int GetArgument(
			int dwIndex,
			[MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

		// HRESULT GetStackDepth(
		//     [out] ULONG32               *pDepth
		// );
		int GetStackDepth();

		// HRESULT GetStackValue(
		//     [in] DWORD                  dwIndex,
		//     [out] ICorDebugValue      **ppValue
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugValue GetStackValue(
			int dwIndex);

		// HRESULT CanSetIP(
		//     [in] ULONG32   nOffset
		// );
		[PreserveSig]
		int CanSetIP(
			int nOffset);
	}
}
