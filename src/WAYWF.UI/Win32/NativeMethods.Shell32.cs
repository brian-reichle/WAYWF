// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.UI.Win32
{
	partial class NativeMethods
	{
		// DWORD_PTR SHGetFileInfo(
		//     [In]    LPCTSTR    pszPath,
		//             DWORD      dwFileAttributes,
		//     [Inout] SHFILEINFO *psfi,
		//             UINT       cbFileInfo,
		//             UINT       uFlags
		// );
		[DllImport("shell32.dll", EntryPoint = "SHGetFileInfoW", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		public static extern IntPtr SHGetFileInfo(
			[MarshalAs(UnmanagedType.LPWStr)] string pszPath,
			FileAttributes dwFileAttributes,
			ref SHFILEINFO psfi,
			int cbFileInfo,
			SHGFI uFlags);
	}
}
