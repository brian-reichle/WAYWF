// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.Core.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("86F012BF-FF15-4372-BD30-B6F11CAAE1DD")]
	interface ICorDebugModule3
	{
		// HRESULT CreateReaderForInMemorySymbols(
		//     [in] REFIID riid,
		//     [out][iid_is(riid)] void **ppObj
		// );
		[PreserveSig]
		int CreateReaderForInMemorySymbols(
			[MarshalAs(UnmanagedType.LPStruct)] Guid riid,
			[MarshalAs(UnmanagedType.Interface, IidParameterIndex = 0)] out object ppObj);
	}
}
