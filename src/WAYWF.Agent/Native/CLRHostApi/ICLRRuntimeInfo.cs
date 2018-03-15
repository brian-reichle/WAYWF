// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;
using WAYWF.Agent.Win32;

namespace WAYWF.Agent.CLRHostApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("BD39D1D2-BA2F-486A-89B0-B4B0CB466891")]
	unsafe interface ICLRRuntimeInfo
	{
		// HRESULT GetVersionString(
		//     [out, size_is(*pcchBuffer)] LPWSTR pwzBuffer,
		//     [in, out] DWORD *pcchBuffer
		// );
		[PreserveSig]
		int GetVersionString(
			char* pwzBuffer,
			ref int pcchBuffer);

		// HRESULT GetRuntimeDirectory(
		//     [out, size_is(*pcchBuffer)] LPWSTR pwzBuffer,
		//     [in, out] DWORD *pcchBuffer
		// );
		void GetRuntimeDirectory(
			char* pwzBuffer,
			ref int pcchBuffer);

		// HRESULT IsLoaded(
		//     [in] HANDLE hndProcess,
		//     [out, retval] BOOL *pbLoaded
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		bool IsLoaded(
			ProcessHandle hndProcess);

		// HRESULT LoadErrorString(
		//     [in] UINT iResourceID,
		//     [out, size_is(*pcchBuffer)] LPWSTR pwzBuffer,
		//     [in, out] DWORD *pcchBuffer,
		//     [in, lcid] LONG iLocaleID
		// );
		void LoadErrorString(
			int iResourceID,
			char* pwzBuffer,
			ref int pcchBuffer,
			int iLocaleID);

		// HRESULT LoadLibrary(
		//     [in]  LPCWSTR pwzDllName,
		//     [out, retval] HMODULE *phndModule
		// );
		IntPtr LoadLibrary(
			[MarshalAs(UnmanagedType.LPWStr)] string pwzDllName);

		// HRESULT GetProcAddress(
		//     [in] LPCSTR pszProcName,
		//     [out, retval] LPVOID *ppProc
		// );
		IntPtr GetProcAddress(
			[MarshalAs(UnmanagedType.LPStr)] string pszProcName);

		// HRESULT GetInterface(
		//     [in] REFCLSID rclsid,
		//     [in] REFIID riid,
		//     [out, iid_is(riid), retval] LPVOID *ppUnk
		// );
		[return: MarshalAs(UnmanagedType.Interface, IidParameterIndex = 1)]
		object GetInterface(
			[MarshalAs(UnmanagedType.LPStruct)] Guid rclsid,
			[MarshalAs(UnmanagedType.LPStruct)] Guid riid);

		// HRESULT IsLoadable(
		//     [out, retval] BOOL *pbLoadable
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		bool IsLoadable();

		// HRESULT SetDefaultStartupFlags(
		//     [in] DWORD dwStartupFlags,
		//     [in] LPCWSTR pwzHostConfigFile
		// );
		void SetDefaultStartupFlags(
			int dwStartupFlags,
			[MarshalAs(UnmanagedType.LPWStr)] string pwzHostConfigFile);

		// HRESULT GetDefaultStartupFlags(
		//     [out] DWORD *pdwStartupFlags,
		//     [out, size_is(*pcchHostConfigFile)] LPWSTR pwzHostConfigFile,
		//     [in, out] DWORD *pcchHostConfigFile
		// );
		void GetDefaultStartupFlags(
			out int pdwStartupFlags,
			char* pwzHostConfigFile,
			ref int pcchHostConfigFile);

		// HRESULT BindAsLegacyV2Runtime();
		void BindAsLegacyV2Runtime();

		// HRESULT IsStarted(
		//     [out] BOOL *pbStarted,
		//     [out] DWORD *pdwStartupFlags
		// );
		void IsStarted(
			[MarshalAs(UnmanagedType.Bool)] out bool pbStarted,
			out int pdwStartupFlags);
	}
}
