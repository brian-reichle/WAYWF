// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.UI.Win32
{
	partial class NativeMethods
	{
		// WINOLEAPI CreateStreamOnHGlobal(
		//     [In]  HGLOBAL  hGlobal,
		//     [In]  BOOL     fDeleteOnRelease,
		//     [Out] LPSTREAM *ppstm
		// );
		[DllImport("ole32.dll")]
		[SuppressUnmanagedCodeSecurity]
		public static extern int CreateStreamOnHGlobal(
			IntPtr hGlobal,
			[MarshalAs(UnmanagedType.Bool)] bool fDeleteOnRelease,
			out IntPtr ppstm);
	}
}
