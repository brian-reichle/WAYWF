// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace WAYWF.UI.Win32
{
	[StructLayout(LayoutKind.Sequential)]
	struct SHFILEINFO
	{
		// HICON hIcon;
		public IntPtr hIcon;

		// int iIcon;
		public int iIcon;

		// DWORD dwAttributes;
		public int dwAttributes;

		// TCHAR szDisplayName[MAX_PATH];
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string szDisplayName;

		// TCHAR szTypeName[80];
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string szTypeName;
	}
}
