// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.Core.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("CC7BCAF9-8A68-11d2-983C-0000F808342D")]
	interface ICorDebugReferenceValue : ICorDebugValue
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

		// HRESULT IsNull(
		//     [out] BOOL *pbNull
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		bool IsNull();

		// HRESULT GetValue(
		//     [out] CORDB_ADDRESS *pValue
		// );
		CORDB_ADDRESS GetValue();

		// HRESULT SetValue(
		//     [in] CORDB_ADDRESS value
		// );
		void SetValue(CORDB_ADDRESS value);

		// HRESULT Dereference(
		//     [out] ICorDebugValue **ppValue
		// );
		[PreserveSig]
		int Dereference(
			[MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

		// HRESULT DereferenceStrong(
		//     [out] ICorDebugValue **ppValue
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugValue DereferenceStrong();
	}
}
