// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("CC7BCAF7-8A68-11D2-983C-0000F808342D")]
	interface ICorDebugValue
	{
		// HRESULT GetType(
		//     [out] CorElementType   *pType
		// );
		CorElementType GetType();

		// HRESULT GetSize(
		//     [out] ULONG32   *pSize
		// );
		int GetSize();

		// HRESULT GetAddress(
		//     [out] CORDB_ADDRESS   *pAddress
		// );
		CORDB_ADDRESS GetAddress();

		// HRESULT CreateBreakpoint(
		//     [out] ICorDebugValueBreakpoint **ppBreakpoint
		// );
		void CreateBreakpoint_();
	}
}
