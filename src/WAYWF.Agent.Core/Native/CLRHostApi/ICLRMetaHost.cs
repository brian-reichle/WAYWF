// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;
using WAYWF.Agent.Core.Win32;

namespace WAYWF.Agent.Core.CLRHostApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("D332DB9E-B9B3-4125-8207-A14884F53216")]
	unsafe interface ICLRMetaHost
	{
		// HRESULT GetRuntime(
		//     [in] LPCWSTR pwzVersion,
		//     [in, REFIID riid,
		//     [out,iid_is(riid), retval] LPVOID *ppRuntime
		// );
		[return: MarshalAs(UnmanagedType.Interface, IidParameterIndex = 1)]
		object GetRuntime(
			[MarshalAs(UnmanagedType.LPWStr)] string pwzVersion,
			[MarshalAs(UnmanagedType.LPStruct)] Guid riid);

		// HRESULT GetVersionFromFile(
		//     [in] LPCWSTR pwzFilePath,
		//     [out, size_is(*pcchBuffer)] LPWSTR pwzBuffer,
		//     [in, out] DWORD *pcchBuffer
		// );
		[PreserveSig]
		int GetVersionFromFile(
			[MarshalAs(UnmanagedType.LPWStr)] string filePath,
			char* buffer,
			ref int bufferLength);

		// HRESULT EnumerateInstalledRuntimes(
		//     [out, retval] IEnumUnknown **ppEnumerator
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		IEnumUnknown EnumerateInstalledRuntimes();

		// HRESULT EnumerateLoadedRuntimes(
		//     [in] HANDLE hndProcess,
		//     [out, retval] IEnumUnknown **ppEnumerator
		// );
		[PreserveSig]
		int EnumerateLoadedRuntimes(
			ProcessHandle hndProcess,
			[MarshalAs(UnmanagedType.Interface)] out IEnumUnknown ppEnumerator);

		// HRESULT RequestRuntimeLoadedNotification(
		//     [in] RuntimeLoadedCallbackFnPtr pCallbackFunction
		// );
		void RequestRuntimeLoadedNotification(
			IntPtr pCallbackFunction);

		// HRESULT QueryLegacyV2RuntimeBinding(
		//     [in] REFIID riid,
		//     [out, iid_is(riid), retval] LPVOID *ppUnk
		// );
		[return: MarshalAs(UnmanagedType.Interface, IidParameterIndex = 0)]
		object QueryLegacyV2RuntimeBinding(
			[MarshalAs(UnmanagedType.LPStruct)] Guid riid);

		// HRESULT ExitProcess(
		//     [in] INT32 iExitCode
		// );
		void ExitProcess(
			int iExitCode);
	}
}
