// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("DF59507C-D47A-459E-BCE2-6427EAC8FD06")]
	unsafe interface ICorDebugAssembly
	{
		// HRESULT GetProcess(
		//     [out] ICorDebugProcess **ppProcess
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugProcess GetProcess();

		// HRESULT GetAppDomain(
		//     [out] ICorDebugAppDomain  **ppAppDomain
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugAppDomain GetAppDomain();

		// HRESULT EnumerateModules(
		//     [out] ICorDebugModuleEnum **ppModules
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugModuleEnum EnumerateModules();

		// HRESULT GetCodeBase(
		//     [in] ULONG32  cchName,
		//     [out] ULONG32 *pcchName,
		//     [out, size_is(cchName), length_is(*pcchName)] WCHAR szName[]
		// );
		void GetCodeBase_();

		// HRESULT GetName(
		//     [in] ULONG32  cchName,
		//     [out] ULONG32 *pcchName,
		//     [out, size_is(cchName), length_is(*pcchName)] WCHAR szName[]
		// );
		void GetName(
			int cchName,
			out int pcchName,
			char* szName);
	}
}
