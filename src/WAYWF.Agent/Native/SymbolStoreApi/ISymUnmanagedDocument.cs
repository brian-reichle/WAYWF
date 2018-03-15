// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.SymbolStoreApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("40DE4037-7C81-3E1E-B022-AE1ABFF2CA08")]
	unsafe interface ISymUnmanagedDocument
	{
		// HRESULT GetURL(
		//     [in]  ULONG32  cchUrl,
		//     [out] ULONG32  *pcchUrl,
		//     [out, size_is(cchUrl), length_is(*pcchUrl)] WCHAR szUrl[]
		// );
		void GetURL(
			int cchUrl,
			out int pcchUrl,
			char* szUrl);

		// HRESULT GetDocumentType(
		//     [out, retval] GUID*  pRetVal
		// );
		void GetDocumentType(
			out Guid pRetVal);

		// HRESULT GetLanguage(
		//     [out, retval]  GUID*  pRetVal
		// );
		void GetLanguage(
			out Guid pRetVal);

		// HRESULT GetLanguageVendor(
		//     [out, retval]  GUID*  pRetVal
		// );
		void GetLanguageVendor(
			out Guid pRetVal);

		// HRESULT GetCheckSumAlgorithmId(
		//     [out, retval] GUID*  pRetVal
		// );
		Guid GetCheckSumAlgorithmId();

		// HRESULT GetCheckSum(
		//     [in]  ULONG32  cData,
		//     [out] ULONG32  *pcData,
		//     [out, size_is(cData), length_is(*pcData)] BYTE data[]
		// );
		void GetCheckSum(
			int cData,
			out int pcData,
			byte* data);

		// HRESULT FindClosestLine(
		//     [in]  ULONG32  line,
		//     [out, retval] ULONG32*  pRetVal
		// );
		int FindClosestLine(
			int line);

		// HRESULT HasEmbeddedSource(
		//    [out, retval]  BOOL  *pRetVal
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		bool HasEmbeddedSource();

		// HRESULT GetSourceLength(
		//     [out, retval]  ULONG32*  pRetVal
		// );
		int GetSourceLength();

		// HRESULT GetSourceRange(
		//     [in]  ULONG32  startLine,
		//     [in]  ULONG32  startColumn,
		//     [in]  ULONG32  endLine,
		//     [in]  ULONG32  endColumn,
		//     [in]  ULONG32  cSourceBytes,
		//     [out] ULONG32  *pcSourceBytes,
		//     [out, size_is(cSourceBytes), length_is(*pcSourceBytes)] BYTE source[]
		// );
		void GetSourceRange(
			int startLine,
			int startColumn,
			int endLine,
			int endColumn,
			int cSourceBytes,
			out int pcSourceBites,
			byte* source);
	}
}
