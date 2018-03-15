// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("5D88A994-6C30-479B-890F-BCEF88B129A5")]
	interface ICorDebugILFrame2
	{
		// HRESULT RemapFunction(
		//     [in] ULONG32      newILOffset
		// );
		void RemapFunction(
			int newILOffset);

		// HRESULT EnumerateTypeParameters(
		//     [out] ICorDebugTypeEnum    **ppTyParEnum
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugTypeEnum EnumerateTypeParameters();
	}
}
