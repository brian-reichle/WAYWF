// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;

namespace WAYWF.Agent.SymbolStoreApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("B4CE6286-2A6B-3712-A3B7-1EE1DAD467B5")]
	unsafe interface ISymUnmanagedReader
	{
		// HRESULT GetDocument(
		//     [in]  WCHAR  *url,
		//     [in]  GUID   language,
		//     [in]  GUID   languageVendor,
		//     [in]  GUID   documentType,
		//     [out, retval] ISymUnmanagedDocument** pRetVal
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ISymUnmanagedDocument GetDocument(
			[MarshalAs(UnmanagedType.LPWStr)] string url,
			Guid language,
			Guid languageVendor,
			Guid documentType);

		// HRESULT GetDocuments(
		//     [in]  ULONG32  cDocs,
		//     [out] ULONG32  *pcDocs,
		//     [out, size_is (cDocs), length_is (*pcDocs)] ISymUnmanagedDocument *pDocs[]
		// );
		void GetDocuments(
			int cDocs,
			out int pcDocs,
			[Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] ISymUnmanagedDocument[] pDocs);

		// HRESULT GetUserEntryPoint(
		//     [out, retval]  mdMethodDef  *pToken
		// );
		MetaDataToken GetUserEntryPoint();

		// HRESULT GetMethod(
		//     [in]  mdMethodDef  token,
		//     [out, retval] ISymUnmanagedMethod**  pRetVal
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ISymUnmanagedMethod GetMethod(
			MetaDataToken methodToken);

		// HRESULT GetMethodByVersion(
		//     [in]  mdMethodDef  token,
		//     [in]  int  version,
		//     [out, retval] ISymUnmanagedMethod** pRetVal
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ISymUnmanagedMethod GetMethodByVersion(
			MetaDataToken token,
			int version);

		// HRESULT GetVariables(
		//     [in]  mdToken  parent,
		//     [in]  ULONG32  cVars,
		//     [out] ULONG32  *pcVars,
		//     [out, size_is (cVars), length_is (*pcVars)] ISymUnmanagedVariable *pVars[]
		// );
		void GetVariables_();

		// HRESULT GetGlobalVariables(
		//     [in]  ULONG32  cVars,
		//     [out] ULONG32  *pcVars,
		//     [out, size_is(cVars), length_is(*pcVars)] ISymUnmanagedVariable *pVars[]
		// );
		void GetGlobalVariables_();

		// HRESULT GetMethodFromDocumentPosition(
		//     [in]  ISymUnmanagedDocument*  document,
		//     [in]  ULONG32  line,
		//     [in]  ULONG32  column,
		//     [out, retval] ISymUnmanagedMethod**  pRetVal
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ISymUnmanagedMethod GetMethodFromDocumentPosition(
			[MarshalAs(UnmanagedType.Interface)] ISymUnmanagedDocument document,
			int line,
			int column);

		// HRESULT GetSymAttribute(
		//     [in]  mdToken  parent,
		//     [in]  WCHAR    *name,
		//     [in]  ULONG32  cBuffer,
		//     [out] ULONG32  *pcBuffer,
		//     [out, size_is (cBuffer), length_is (*pcBuffer)] BYTE buffer[]
		// );
		void GetSymAttribute(
			MetaDataToken parent,
			[MarshalAs(UnmanagedType.LPWStr)] string name,
			int cBuffer,
			out int pcBuffer,
			[Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] buffer);

		// HRESULT GetNamespaces(
		//     [in]  ULONG32  cNameSpaces,
		//     [out] ULONG32  *pcNameSpaces,
		//     [out, size_is (cNameSpaces), length_is (*pcNameSpaces)] ISymUnmanagedNamespace*  namespaces[]
		// );
		void GetNamespaces_();

		// HRESULT Initialize(
		//     [in]  IUnknown     *importer,
		//     [in]  const WCHAR  *filename,
		//     [in]  const WCHAR  *searchPath,
		//     [in]  IStream      *pIStream
		// );
		void Initialize(
			[MarshalAs(UnmanagedType.IUnknown)] object importer,
			[MarshalAs(UnmanagedType.LPWStr)] string filename,
			[MarshalAs(UnmanagedType.LPWStr)] string searchPath,
			[MarshalAs(UnmanagedType.Interface)] IStream pIStream);

		// HRESULT UpdateSymbolStore(
		//     [in] const WCHAR *filename,
		//     [in] IStream *pIStream
		// );
		void UpdateSymbolStore(
			[MarshalAs(UnmanagedType.LPWStr)] string filename,
			[MarshalAs(UnmanagedType.Interface)] IStream pIStream);

		// HRESULT ReplaceSymbolStore(
		//     [in] const WCHAR *filename,
		//     [in] IStream *pIStream
		// );
		void ReplaceSymbolStore(
			[MarshalAs(UnmanagedType.LPWStr)] string filename,
			[MarshalAs(UnmanagedType.Interface)] IStream pIStream);

		// HRESULT GetSymbolStoreFileName(
		//     [in]  ULONG32 cchName,
		//     [out] ULONG32 *pcchName,
		//     [out, size_is (cchName), length_is (*pcchName)] WCHAR szName[]
		// );
		void GetSymbolStoreFileName(
			int cchName,
			out int pcchName,
			char* szName);

		// HRESULT GetMethodsFromDocumentPosition(
		//     [in]  ISymUnmanagedDocument* document,
		//     [in]  ULONG32 line,
		//     [in]  ULONG32 column,
		//     [in]  ULONG32 cMethod,
		//     [out] ULONG32* pcMethod,
		//     [out, size_is (cMethod), length_is (*pcMethod)] ISymUnmanagedMethod* pRetVal[]
		// );
		void GetMethodsFromDocumentPosition(
			[MarshalAs(UnmanagedType.Interface)] ISymUnmanagedDocument document,
			int line,
			int column,
			int cMethod,
			out int pcMethod,
			[Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] ISymUnmanagedMethod[] pRetVal);

		// HRESULT GetDocumentVersion(
		//     [in]  ISymUnmanagedDocument *pDoc,
		//     [out] int* version,
		//     [out] BOOL* pbCurrent
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		bool GetDocumentVersion(
			[MarshalAs(UnmanagedType.Interface)] ISymUnmanagedDocument pDoc,
			out int version);

		// HRESULT GetMethodVersion(
		//     [in]  ISymUnmanagedMethod* pMethod,
		//     [out] int* version
		// );
		int GetMethodVersion(
			[MarshalAs(UnmanagedType.Interface)] ISymUnmanagedMethod pMethod);
	}
}
