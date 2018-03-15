// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("0405B0DF-A660-11d2-BD02-0000F80849BD")]
	interface ICorDebugArrayValue : ICorDebugHeapValue
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

		// HRESULT GetElementType(
		//     [out] CorElementType  *pType
		// );
		CorElementType GetElementType();

		// HRESULT GetRank(
		//     [out] ULONG32   *pnRank
		// );
		int GetRank();

		// HRESULT GetCount(
		//     [out] ULONG32 *pnCount
		// );
		int GetCount();

		// HRESULT GetDimensions(
		//     [in] ULONG32         cdim,
		//     [out, size_is(cdim), length_is(cdim)] ULONG32          dims[]
		// );
		void GetDimensions(
			int cdim,
			[MarshalAs(UnmanagedType.LPArray)] int[] dims);

		// HRESULT HasBaseIndicies(
		//     [out] BOOL    *pbHasBaseIndicies
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		bool HasBaseIndicies();

		// HRESULT GetBaseIndicies(
		//     [in] ULONG32          cdim,
		//     [out, size_is(cdim), length_is(cdim)] ULONG32           indicies[]
		// );
		void GetBaseIndicies(
			int cdim,
			[MarshalAs(UnmanagedType.LPArray)] int[] indicies);

		// HRESULT GetElement(
		//     [in]  ULONG32          cdim,
		//     [in, size_is(cdim), length_is(cdim)] ULONG32           indices[],
		//     [out] ICorDebugValue   **ppValue
		// );
		void GetElement_();

		// HRESULT GetElementAtPosition(
		//     [in]  ULONG32          nPosition,
		//     [out] ICorDebugValue   **ppValue
		// );
		void GetElementAtPosition_();
	}
}
