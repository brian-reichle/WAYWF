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
	[Guid("D613F0BB-ACE1-4C19-BD72-E4C08D5DA7F5")]
	interface ICorDebugType
	{
		// HRESULT GetType(
		//     [out] CorElementType   *ty
		// );
		CorElementType GetType();

		// HRESULT GetClass(
		//     [out] ICorDebugClass   **ppClass
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugClass GetClass();

		// HRESULT EnumerateTypeParameters(
		//     [out] ICorDebugTypeEnum   **ppTyParEnum
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugTypeEnum EnumerateTypeParameters();

		// HRESULT GetFirstTypeParameter(
		//     [out] ICorDebugType   **value
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugType GetFirstTypeParameter();

		// HRESULT GetBase(
		//     [out] ICorDebugType   **pBase
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugType GetBase();

		// HRESULT GetStaticFieldValue(
		//     [in]  mdFieldDef        fieldDef,
		//     [in]  ICorDebugFrame    *pFrame,
		//     [out] ICorDebugValue    **ppValue
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugValue GetStaticFieldValue(
			MetaDataToken fieldDef,
			[MarshalAs(UnmanagedType.Interface)] ICorDebugFrame pFrame);

		// HRESULT GetRank(
		//     [out] ULONG32   *pnRank
		// );
		int GetRank();
	}
}
