// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;
using WAYWF.Agent.Data;

namespace WAYWF.Agent.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("DBA2D8C1-E5C5-4069-8C13-10A7C6ABF43D")]
	unsafe interface ICorDebugModule
	{
		// HRESULT GetProcess(
		//     [out] ICorDebugProcess **ppProcess
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugProcess GetProcess();

		// HRESULT GetBaseAddress(
		//     [out] CORDB_ADDRESS *pAddress
		// );
		CORDB_ADDRESS GetBaseAddress();

		// HRESULT GetAssembly(
		//     [out] ICorDebugAssembly **ppAssembly
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugAssembly GetAssembly();

		// HRESULT GetName(
		//     [in] ULONG32 cchName,
		//     [out] ULONG32 *pcchName,
		//     [out, size_is(cchName), length_is(*pcchName)] WCHAR szName[]
		// );
		void GetName(
			int cchName,
			out int pcchName,
			char* szName);

		// HRESULT EnableJITDebugging(
		//     [in] BOOL bTrackJITInfo,
		//     [in] BOOL bAllowJitOpts
		// );
		void EnableJITDebugging(
			[MarshalAs(UnmanagedType.Bool)] bool bTrackJITInfo,
			[MarshalAs(UnmanagedType.Bool)] bool bAllowJitOpts);

		// HRESULT EnableClassLoadCallbacks(
		//     [in] BOOL bClassLoadCallbacks
		// );
		void EnableClassLoadCallbacks(
			[MarshalAs(UnmanagedType.Bool)] bool bClassLoadCallbacks);

		// HRESULT GetFunctionFromToken(
		//     [in] mdMethodDef methodDef,
		//     [out] ICorDebugFunction **ppFunction
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugFunction GetFunctionFromToken(
			MetaDataToken methodDef);

		// HRESULT GetFunctionFromRVA(
		//     [in]  CORDB_ADDRESS rva,
		//     [out] ICorDebugFunction **ppFunction
		// );
		ICorDebugFunction GetFunctionFromRVA(
			CORDB_ADDRESS rva);

		// HRESULT GetClassFromToken(
		//     [in]  mdTypeDef typeDef,
		//     [out] ICorDebugClass **ppClass
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugClass GetClassFromToken(
			MetaDataToken typeDef);

		// HRESULT CreateBreakpoint(
		//     [out] ICorDebugModuleBreakpoint **ppBreakpoint
		// );
		void CreateBreakpoint_();

		// HRESULT GetEditAndContinueSnapshot(
		//     [out] ICorDebugEditAndContinueSnapshot **ppEditAndContinueSnapshot
		// );
		void GetEditAndContinueSnapshot_();

		// HRESULT GetMetaDataInterface(
		//     [in] REFIID riid,
		//     [out] IUnknown **ppObj
		// );
		[return: MarshalAs(UnmanagedType.IUnknown, IidParameterIndex = 0)]
		object GetMetaDataInterface(
			[MarshalAs(UnmanagedType.LPStruct)] Guid riid);

		// HRESULT GetToken(
		//     [out] mdModule *pToken
		// );
		MetaDataToken GetToken();

		// HRESULT IsDynamic(
		//     [out] BOOL *pDynamic
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		bool IsDynamic();

		// HRESULT GetGlobalVariableValue(
		//     [in]  mdFieldDef      fieldDef,
		//     [out] ICorDebugValue  **ppValue
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugValue GetGlobalVariableValue(
			MetaDataToken fieldDef);

		// HRESULT GetSize(
		//     [out] ULONG32 *pcBytes
		// );
		int GetSize();

		// HRESULT IsInMemory(
		//     [out] BOOL *pInMemory
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		bool IsInMemory();
	}
}
