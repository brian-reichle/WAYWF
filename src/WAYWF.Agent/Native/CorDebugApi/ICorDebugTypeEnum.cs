// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("10F27499-9DF2-43CE-8333-A321D7C99CB4")]
	interface ICorDebugTypeEnum : ICorDebugEnum
	{
		// HRESULT Skip(
		//     [in] ULONG celt
		// );
		new void Skip(
			int celt);

		// HRESULT Reset();
		new void Reset();

		// HRESULT Clone(
		//     [out] ICorDebugEnum **ppEnum
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		new ICorDebugEnum Clone();

		// HRESULT GetCount(
		//     [out] ULONG *pcelt
		// );
		new int GetCount();

		// HRESULT Next (
		//     [in]  ULONG  celt,
		//     [out, size_is(celt), length_is(*pceltFetched)] ICorDebugType *values[],
		//     [out] ULONG *pceltFetched
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		bool Next(
			int celt,
			[MarshalAs(UnmanagedType.Interface)] out ICorDebugType values);
	}
}
