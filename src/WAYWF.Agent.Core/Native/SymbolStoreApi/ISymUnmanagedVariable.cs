// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.Core.SymbolStoreApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("9f60eebe-2d9a-3f7c-bf58-80bc991c60bb")]
	unsafe interface ISymUnmanagedVariable
	{
		// HRESULT GetName(
		//     [in]  ULONG32  cchName,
		//     [out] ULONG32  *pcchName,
		//     [out, size_is(cchName), length_is(*pcchName)] WCHAR szName[]
		// );
		void GetName(
			int cchName,
			out int pcchName,
			char* szName);

		// HRESULT GetAttributes(
		//     [out, retval] ULONG32* pRetVal
		// );
		void GetAttributes_();

		// HRESULT GetSignature(
		//     [in]  ULONG32  cSig,
		//     [out] ULONG32  *pcSig,
		//     [out, size_is(cSig), length_is(*pcSig)] BYTE sig[]
		// );
		void GetSignature_();

		// HRESULT GetAddressKind(
		//     [out, retval] ULONG32* pRetVal
		// );
		CorSymAddrKind GetAddressKind();

		// HRESULT GetAddressField1(
		//     [out, retval] ULONG32* pRetVal
		// );
		int GetAddressField1();

		// HRESULT GetAddressField2(
		//     [out, retval] ULONG32* pRetVal
		// );
		void GetAddressField2_();

		// HRESULT GetAddressField3(
		//     [out, retval] ULONG32* pRetVal
		// );
		void GetAddressField3_();

		// HRESULT GetStartOffset(
		//     [out, retval] ULONG32* pRetVal
		// );
		void GetStartOffset_();

		// HRESULT GetEndOffset(
		//     [out, retval] ULONG32* pRetVal
		// );
		void GetEndOffset_();
	}
}
