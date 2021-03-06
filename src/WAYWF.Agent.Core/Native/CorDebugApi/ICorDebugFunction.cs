// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;
using WAYWF.Agent.Data;

namespace WAYWF.Agent.Core.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("CC7BCAF3-8A68-11D2-983C-0000F808342D")]
	interface ICorDebugFunction
	{
		// HRESULT GetModule(
		//     [out] ICorDebugModule **ppModule
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugModule GetModule();

		// HRESULT GetClass(
		//     [out] ICorDebugClass **ppClass
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugClass GetClass();

		// HRESULT GetToken(
		//     [out] mdMethodDef *pMethodDef
		// );
		MetaDataToken GetToken();

		// HRESULT GetILCode(
		//     [out] ICorDebugCode **ppCode
		// );
		void GetILCode_();

		// HRESULT GetNativeCode(
		//     [out] ICorDebugCode **ppCode
		// );
		void GetNativeCode_();

		// HRESULT CreateBreakpoint(
		//     [out] ICorDebugFunctionBreakpoint **ppBreakpoint
		// );
		void CreateBreakpoint_();

		// HRESULT GetLocalVarSigToken(
		//     [out] mdSignature *pmdSig
		// );
		MetaDataToken GetLocalVarSigToken();

		// HRESULT GetCurrentVersionNumber(
		//     [out] ULONG32 *pnCurrentVersion
		// );
		int GetCurrentVersionNumber();
	}
}
