// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;

namespace WAYWF.Agent.Core.SymbolStoreApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("AA544d42-28CB-11d3-bd22-0000f80849bd")]
	interface ISymUnmanagedBinder
	{
		// HRESULT GetReaderForFile(
		//     [in]  IUnknown     *importer,
		//     [in]  const WCHAR  *fileName,
		//     [in]  const WCHAR  *searchPath,
		//     [out, retval] ISymUnmanagedReader  **pRetVal
		// );
		[PreserveSig]
		int GetReaderForFile(
			[MarshalAs(UnmanagedType.IUnknown)] object importer,
			[MarshalAs(UnmanagedType.LPWStr)] string filename,
			[MarshalAs(UnmanagedType.LPWStr)] string searchPath,
			[MarshalAs(UnmanagedType.Interface)] out ISymUnmanagedReader retVal);

		// HRESULT GetReaderFromStream(
		//     [in]  IUnknown  *importer,
		//     [in]  IStream   *pstream,
		//     [out,retval] ISymUnmanagedReader **pRetVal
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ISymUnmanagedReader GetReaderFromStream(
			[MarshalAs(UnmanagedType.IUnknown)] object importer,
			[MarshalAs(UnmanagedType.Interface)] IStream pstream);
	}
}
