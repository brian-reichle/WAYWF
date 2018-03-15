// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.UI.Win32
{
	static partial class NativeMethods
	{
		// HGLOBAL WINAPI GlobalAlloc(
		//     [In] UINT   uFlags,
		//     [In] SIZE_T dwBytes
		// );
		[DllImport("kernel32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		public static extern SafeHGlobal GlobalAlloc(
			int uFlags,
			IntPtr dwBytes);

		// HGLOBAL WINAPI GlobalFree(
		//     [In] HGLOBAL hMem
		// );
		[DllImport("kernel32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		public static extern IntPtr GlobalFree(
			IntPtr hMem);

		// LPVOID WINAPI GlobalLock(
		//     [In] HGLOBAL hMem
		// );
		[DllImport("kernel32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		public static extern SafeHGlobalBuffer GlobalLock(
			SafeHGlobal hMem);

		// BOOL WINAPI GlobalUnlock(
		//     [In] HGLOBAL hMem
		// );
		[DllImport("kernel32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GlobalUnlock(
			SafeHGlobal hMem);
	}
}
