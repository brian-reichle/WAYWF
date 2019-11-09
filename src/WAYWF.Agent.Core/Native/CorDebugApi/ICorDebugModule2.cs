// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;
using WAYWF.Agent.Data;

namespace WAYWF.Agent.Core.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("7FCC5FB5-49C0-41DE-9938-3B88B5B9ADD7")]
	interface ICorDebugModule2
	{
		// HRESULT SetJMCStatus(
		//     [in] BOOL                        bIsJustMyCode,
		//     [in] ULONG32                     cTokens,
		//     [in, size_is(cTokens)] mdToken   pTokens[]
		// );
		void SetJMCStatus_();

		// HRESULT ApplyChanges(
		//     [in] ULONG                       cbMetadata,
		//     [in, size_is(cbMetadata)] BYTE   pbMetadata[],
		//     [in] ULONG                       cbIL,
		//     [in, size_is(cbIL)] BYTE         pbIL[]
		// );
		void ApplyChanges_();

		// HRESULT SetJITCompilerFlags(
		//     [in] DWORD dwFlags
		// );
		void SetJITCompilerFlags_();

		// HRESULT GetJITCompilerFlags(
		//     [out] DWORD   *pdwFlags
		// );
		void GetJITCompilerFlags_();

		// HRESULT ResolveAssembly(
		//     [in]  mdToken             tkAssemblyRef,
		//     [out] ICorDebugAssembly   **ppAssembly
		// );
		[PreserveSig]
		int ResolveAssembly(
			MetaDataToken tkAssemblyRef,
			[MarshalAs(UnmanagedType.Interface)] out ICorDebugAssembly ppAssembly);
	}
}
