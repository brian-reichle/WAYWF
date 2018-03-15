// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.UI.Win32
{
	partial class NativeMethods
	{
		// BOOL WINAPI DestroyIcon(
		//     [In] HICON hIcon
		// );
		[DllImport("user32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DestroyIcon(
			IntPtr hIcon);

		// DWORD GetWindowThreadProcessId(
		//     [in]  HWND    hWnd,
		//     [out] LPDWORD lpdwProcessId
		// );
		[DllImport("user32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		public static extern uint GetWindowThreadProcessId(
			IntPtr hWnd,
			out int lpdwProcessId);

		// HWND WindowFromPoint(
		//     [in]  POINT Point
		// );
		[SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "0")]
		[DllImport("user32.dll")]
		[SuppressUnmanagedCodeSecurity]
		public static extern IntPtr WindowFromPoint(
			POINT Point);
	}
}
