// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.Core.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("3D6F5F61-7538-11D3-8D5B-00104B35E7EF")]
	interface ICorDebug
	{
		// HRESULT Initialize();
		void Initialize();

		// HRESULT Terminate();
		void Terminate();

		// HRESULT SetManagedHandler(
		//     [in] ICorDebugManagedCallback *pCallback
		// );
		void SetManagedHandler(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugManagedCallback pCallback);

		// HRESULT SetUnmanagedHandler(
		//     [in] ICorDebugUnmanagedCallback *pCallback
		// );
		void SetUnmanagedHandler_();

		// HRESULT CreateProcess(
		//     [in]  LPCWSTR                     lpApplicationName,
		//     [in]  LPWSTR                      lpCommandLine,
		//     [in]  LPSECURITY_ATTRIBUTES       lpProcessAttributes,
		//     [in]  LPSECURITY_ATTRIBUTES       lpThreadAttributes,
		//     [in]  BOOL                        bInheritHandles,
		//     [in]  DWORD                       dwCreationFlags,
		//     [in]  PVOID                       lpEnvironment,
		//     [in]  LPCWSTR                     lpCurrentDirectory,
		//     [in]  LPSTARTUPINFOW              lpStartupInfo,
		//     [in]  LPPROCESS_INFORMATION       lpProcessInformation,
		//     [in]  CorDebugCreateProcessFlags  debuggingFlags,
		//     [out] ICorDebugProcess          **ppProcess
		// );
		void CreateProcess_();

		// HRESULT DebugActiveProcess(
		//     [in]  DWORD               id,
		//     [in]  BOOL                win32Attach,
		//     [out] ICorDebugProcess  **ppProcess
		// );
		[PreserveSig]
		int DebugActiveProcess(
			int id,
			[MarshalAs(UnmanagedType.Bool)] bool win32Attach,
			out ICorDebugProcess process);

		// HRESULT EnumerateProcesses(
		//     [out] ICorDebugProcessEnum **ppProcess
		// );
		void EnumerateProcess_();

		// HRESULT GetProcess(
		//     [in] DWORD               dwProcessId,
		//     [out] ICorDebugProcess **ppProcess
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugProcess GetProcess(
			int dwProcessId);

		// HRESULT CanLaunchOrAttach(
		//     [in] DWORD dwProcessId,
		//     [in] BOOL  win32DebuggingEnabled
		// );
		[PreserveSig]
		int CanLaunchOrAttach(
			int dwProcessId,
			[MarshalAs(UnmanagedType.Bool)] bool win32DebuggingEnabled = false);
	}
}
