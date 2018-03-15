// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.Win32
{
	partial class NativeMethods
	{
		// BOOL CloseHandle(
		//     [in] HANDLE hObject
		// );
		[DllImport("kernel32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CloseHandle(
			IntPtr handle);

		// BOOL WINAPI GetExitCodeProcess(
		//     [in]  HANDLE  hProcess,
		//     [out] LPDWORD lpExitCode
		// );
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetExitCodeProcess(
			ProcessHandle hProcess,
			out int lpExitCode);

		// DWORD WINAPI GetModuleBaseName(
		//     [In]     HANDLE  hProcess,
		//     [In,opt] HMODULE hModule,
		//     [Out]    LPTSTR  lpBaseName,
		//     [In]     DWORD   nSize
		// );
		[DllImport("kernel32.dll", EntryPoint = "K32GetModuleBaseName", SetLastError = true)]
		public static extern int GetModuleBaseName(
			ProcessHandle hProcess,
			IntPtr hModule,
			[Out, MarshalAs(UnmanagedType.LPArray)] char[] lpBaseName,
			int nSize);

		// BOOL WINAPI IsWow64Process(
		//     [in]   HANDLE      hProcess,
		//     [out]  PBOOL       Wow64Process
		// );
		[DllImport("kernel32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWow64Process(
			ProcessHandle hProcess,
			[MarshalAs(UnmanagedType.Bool)] out bool Wow64Process);

		// HANDLE OpenProcess(
		//     [in]  DWORD dwDesiredAccess,
		//     [in]  BOOL  bInheritHandle,
		//     [in]  DWORD dwProcessId
		// );
		[DllImport("kernel32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		public static extern ProcessHandle OpenProcess(
			AccessOptions dwDesiredAccess,
			[MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
			int dwProcessId);

		// BOOL WINAPI QueryFullProcessImageName(
		//     [in]     HANDLE hProcess,
		//     [in]     DWORD  dwFlags,
		//     [out]    LPTSTR lpExeName,
		//     [in,out] PDWORD lpdwSize
		// );
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool QueryFullProcessImageName(
			ProcessHandle hProcess,
			int dwFlags,
			[Out, MarshalAs(UnmanagedType.LPArray)] char[] lpExeName,
			ref int lpdwSize);

		// SIZE_T WINAPI VirtualQueryEx(
		//     [In]     HANDLE                    hProcess,
		//     [In,opt] LPCVOID                   lpAddress,
		//     [Out]    PMEMORY_BASIC_INFORMATION lpBuffer,
		//     [In]     SIZE_T                    dwLength
		// );
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr VirtualQueryEx(
			ProcessHandle hProcess,
			IntPtr lpAddress,
			out MEMORY_BASIC_INFORMATION lpBuffer,
			IntPtr dwLength);
	}
}
