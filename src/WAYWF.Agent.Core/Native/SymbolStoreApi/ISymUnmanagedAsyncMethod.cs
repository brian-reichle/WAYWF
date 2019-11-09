// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;
using WAYWF.Agent.Data;

namespace WAYWF.Agent.Core.SymbolStoreApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("B20D55B3-532E-4906-87E7-25BD5734ABD2")]
	interface ISymUnmanagedAsyncMethod
	{
		// HRESULT IsAsyncMethod(
		//     [out, retval] BOOL* pRetVal
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		bool IsAsyncMethod();

		// HRESULT GetKickoffMethod(
		//     [out, retval] mdToken* kickoffMethod
		// );
		void GetKickoffMethod_();

		// HRESULT HasCatchHandlerILOffset(
		//     [out, retval] BOOL* pRetVal
		// );
		void HasCatchHandlerILOffset_();

		// HRESULT GetCatchHandlerILOffset(
		//     [out, retval] ULONG32* pRetVal
		// );
		void GetCatchHandlerILOffset_();

		// HRESULT GetAsyncStepInfoCount(
		//     [out, retval] ULONG32* pRetVal
		// );
		int GetAsyncStepInfoCount();

		// HRESULT GetAsyncStepInfo(
		//     [in] ULONG32 cStepInfo,
		//     [out] ULONG32 *pcStepInfo,
		//     [in, size_is(cStepInfo)] ULONG32 yieldOffsets[],
		//     [in, size_is(cStepInfo)] ULONG32 breakpointOffset[],
		//     [in, size_is(cStepInfo)] mdToken breakpointMethod[]
		// );
		void GetAsyncStepInfo(
			int cStepInfo,
			out int pcStepInfo,
			[Out, MarshalAs(UnmanagedType.LPArray)] int[] yieldOffsets,
			[Out, MarshalAs(UnmanagedType.LPArray)] int[] breakpointOffset,
			[Out, MarshalAs(UnmanagedType.LPArray)] MetaDataToken[] breakpointMethod);
	}
}
