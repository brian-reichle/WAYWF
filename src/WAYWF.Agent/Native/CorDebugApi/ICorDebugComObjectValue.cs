// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("5F69C5E5-3E12-42DF-B371-F9D761D6EE24")]
	interface ICorDebugComObjectValue
	{
		// HRESULT GetCachedInterfaceTypes(
		//     [in] BOOL bIInspectableOnly,
		//     [out] ICorDebugTypeEnum **ppInterfacesEnum
		// );
		ICorDebugTypeEnum GetCachedInterfaceTypes(
			[MarshalAs(UnmanagedType.Bool)] bool bIInspectableOnly = false);

		// HRESULT GetCachedInterfacePointers(
		//     [in] BOOL bIInspectableOnly,
		//     [in] ULONG32 celt,
		//     [out] ULONG32 *pceltFetched,
		//     [out, size_is(celt), length_is(*pceltFetched) CORDB_ADDRESS *ptrs
		// );
		void GetCachedInterfacePointers(
			[MarshalAs(UnmanagedType.Bool)] bool bIInspectableOnly,
			int celt,
			out int pceltFetched,
			[Out, MarshalAs(UnmanagedType.LPArray)] CORDB_ADDRESS[] ptrs);
	}
}
