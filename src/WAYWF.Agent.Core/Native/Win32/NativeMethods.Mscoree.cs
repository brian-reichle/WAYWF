// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.Core.Win32
{
	partial class NativeMethods
	{
		// HRESULT CLRCreateInstance(
		//     [in]  REFCLSID  clsid,
		//     [in]  REFIID    riid,
		//     [out] LPVOID   *ppInterface
		// );
		[DllImport("mscoree.dll")]
		[SuppressUnmanagedCodeSecurity]
		public static extern int CLRCreateInstance(
			[MarshalAs(UnmanagedType.LPStruct)] Guid clsid,
			[MarshalAs(UnmanagedType.LPStruct)] Guid riid,
			[MarshalAs(UnmanagedType.Interface)] out object metahostInterface);
	}
}
