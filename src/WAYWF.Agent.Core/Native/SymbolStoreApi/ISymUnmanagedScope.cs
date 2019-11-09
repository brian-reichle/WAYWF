// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.Core.SymbolStoreApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("68005d0f-b8e0-3b01-84d5-a11a94154942")]
	interface ISymUnmanagedScope
	{
		// HRESULT GetMethod(
		//     [out, retval] ISymUnmanagedMethod** pRetVal
		// );
		void GetMethod_();

		// HRESULT GetParent(
		//     [out, retval] ISymUnmanagedScope** pRetVal
		// );
		ISymUnmanagedScope GetParent();

		// HRESULT GetChildren(
		//     [in]  ULONG32  cChildren,
		//     [out] ULONG32  *pcChildren,
		//     [out, size_is(cChildren), length_is(*pcChildren)] ISymUnmanagedScope* children[]
		// );
		void GetChildren(
			int cChildren,
			out int pcChildren,
			[Out, MarshalAs(UnmanagedType.LPArray)] ISymUnmanagedScope[] children);

		// HRESULT GetStartOffset(
		//     [out, retval] ULONG32* pRetVal
		// );
		int GetStartOffset();

		// HRESULT GetEndOffset(
		//     [out, retval] ULONG32* pRetVal
		// );
		int GetEndOffset();

		// HRESULT GetLocalCount(
		//     [out, retval] ULONG32 *pRetVal
		// );
		int GetLocalCount();

		// HRESULT GetLocals(
		//     [in]  ULONG32  cLocals,
		//     [out] ULONG32  *pcLocals,
		//     [out, size_is(cLocals), length_is(*pcLocals)] ISymUnmanagedVariable* locals[]
		// );
		void GetLocals(
			int cLocals,
			out int pcLocals,
			[Out, MarshalAs(UnmanagedType.LPArray)] ISymUnmanagedVariable[] locals);

		// HRESULT GetNamespaces(
		//     [in]  ULONG32  cNameSpaces,
		//     [out] ULONG32  *pcNameSpaces,
		//     [out, size_is(cNameSpaces), length_is(*pcNameSpaces)] ISymUnmanagedNamespace* namespaces[]
		// );
		void GetNamespaces_();
	}
}
