// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("18AD3D6E-B7D2-11d2-BD04-0000F80849BD")]
	interface ICorDebugObjectValue : ICorDebugValue
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

		// HRESULT GetClass(
		//     [out] ICorDebugClass **ppClass
		// );
		ICorDebugClass GetClass();

		// HRESULT GetFieldValue(
		//     [in] ICorDebugClass *pClass,
		//     [in] mdFieldDef fieldDef,
		//     [out] ICorDebugValue **ppValue
		// );
		[PreserveSig]
		int GetFieldValue(
			ICorDebugClass pClass,
			MetaDataToken fieldDef,
			out ICorDebugValue ppValue);

		// HRESULT GetVirtualMethod(
		//     [in] mdMemberRef memberRef,
		//     [out] ICorDebugFunction **ppFunction
		// );
		ICorDebugFunction GetVirtualMethod(
			MetaDataToken memberRef);

		// HRESULT GetContext(
		//     [out] ICorDebugContext **ppContext
		// );
		void GetContext_();

		// HRESULT IsValueClass(
		//     [out] BOOL *pbIsValueClass
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		bool IsValueClass();

		// HRESULT GetManagedCopy(
		//     [out] IUnknown **ppObject
		// );
		void GetManagedCopy_();

		// HRESULT SetFromManagedCopy(
		//     [in] IUnknown *pObject
		// );
		void SetFromManagedCopy_();
	}
}
