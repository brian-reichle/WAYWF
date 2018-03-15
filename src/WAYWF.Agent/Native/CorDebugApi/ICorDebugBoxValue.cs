// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("CC7BCAFC-8A68-11D2-983C-0000F808342D")]
	interface ICorDebugBoxValue : ICorDebugHeapValue
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

		// HRESULT GetObject(
		//     [out] ICorDebugObjectValue **ppObject
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugValue GetObject();
	}
}
