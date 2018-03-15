// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("CC7BCAFD-8A68-11d2-983C-0000F808342D")]
	unsafe interface ICorDebugStringValue : ICorDebugHeapValue
	{
		// HRESULT GetType(
		//     [out] CorElementType   *pType
		// );
		new CorElementType GetType();

		// HRESULT GetSize(
		//     [out] ULONG32   *pSize
		// );
		new int GetSize();

		// HRESULT GetAddress(
		//     [out] CORDB_ADDRESS   *pAddress
		// );
		new CORDB_ADDRESS GetAddress();

		// HRESULT CreateBreakpoint(
		//     [out] ICorDebugValueBreakpoint **ppBreakpoint
		// );
		new void CreateBreakpoint_();

		// HRESULT IsValid(
		//     [out] BOOL    *pbValid
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		new bool IsValid();

		// HRESULT CreateRelocBreakpoint(
		//     [out] ICorDebugValueBreakpoint **ppBreakpoint
		// );
		new void CreateRelocBreakpoint_();

		// HRESULT GetLength(
		//     [out] ULONG32 *pcchString
		// );
		int GetLength();

		// HRESULT GetString(
		//     [in] ULONG32 cchString,
		//     [out] ULONG32 *pcchString,
		//     [out, size_is(cchString), length_is(*pcchString)] WCHAR szString[]
		// );
		void GetString(
			int cchString,
			out int pcchString,
			char* szString);
	}
}
