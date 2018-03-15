// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("CC7BCAF5-8A68-11D2-983C-0000F808342D")]
	interface ICorDebugClass
	{
		// HRESULT GetModule(
		//     [out] ICorDebugModule    **pModule
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugModule GetModule();

		// HRESULT GetToken(
		//     [out] mdTypeDef          *pTypeDef
		// );
		MetaDataToken GetToken();

		// HRESULT GetStaticFieldValue(
		//     [in]  mdFieldDef         fieldDef,
		//     [in]  ICorDebugFrame     *pFrame,
		//     [out] ICorDebugValue     **ppValue
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugValue GetStaticFieldValue(
			MetaDataToken fieldDef,
			[MarshalAs(UnmanagedType.Interface)] ICorDebugFrame pFrame);
	}
}
