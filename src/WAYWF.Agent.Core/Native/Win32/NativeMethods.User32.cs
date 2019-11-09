// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace WAYWF.Agent.Core.Win32
{
	partial class NativeMethods
	{
		// BOOL EnumWindowsProc(
		//     [in]  HWND hwnd,
		//     [in]  LPARAM lParam
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		public delegate bool EnumWindowsProc(
			IntPtr hwnd,
			IntPtr lParam);

		// BOOL EnumWindows(
		//     [in]  WNDENUMPROC lpEnumFunc,
		//     [in]  LPARAM lParam
		// );
		[DllImport("user32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool EnumWindows(
			EnumWindowsProc lpEnumFunc,
			IntPtr lParam);

		// int WINAPI GetClassName(
		//     [In]  HWND   hWnd,
		//     [Out] LPTSTR lpClassName,
		//     [In]  int    nMaxCount
		// );
		[DllImport("user32.dll", SetLastError = true, EntryPoint = "GetClassNameW")]
		[SuppressUnmanagedCodeSecurity]
		public static extern int GetClassName(
			IntPtr hWnd,
			[MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpClassName,
			int nMaxCount);

		// HWND WINAPI GetWindow(
		//     _In_ HWND hWnd,
		//     _In_ UINT uCmd
		// );
		[DllImport("user32.dll", SetLastError = false)]
		[SuppressUnmanagedCodeSecurity]
		public static extern IntPtr GetWindow(
			IntPtr hWnd,
			int uCmd);

		// int GetWindowText(
		//     [in]   HWND hWnd,
		//     [out]  LPTSTR lpString,
		//     [in]   int nMaxCount
		// );
		[DllImport("user32.dll", EntryPoint = "GetWindowTextW", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		public static extern int GetWindowText(
			IntPtr hWnd,
			[MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpString,
			int nMaxCount);

		// int WINAPI GetWindowTextLength(
		//     [in]  HWND hWnd
		// );
		[DllImport("user32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		public static extern int GetWindowTextLength(
			IntPtr hWnd);

		// DWORD GetWindowThreadProcessId(
		//     [in]       HWND hWnd,
		//     [out,opt]  LPDWORD lpdwProcessId
		// );
		[DllImport("user32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		public static extern int GetWindowThreadProcessId(
			IntPtr hWnd,
			out int lpwProcessId);

		// BOOL WINAPI IsWindowEnabled(
		//     [In] HWND hWnd
		// );
		[DllImport("user32.dll")]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWindowEnabled(
			IntPtr hwnd);

		// BOOL IsWindowVisible(
		//     [in]  HWND hWnd
		// );
		[DllImport("user32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWindowVisible(
			IntPtr hwnd);
	}
}
