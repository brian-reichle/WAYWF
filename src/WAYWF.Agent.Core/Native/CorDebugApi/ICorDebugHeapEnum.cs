// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.Core.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("76D7DAB8-D044-11DF-9A15-7E29DFD72085")]
	interface ICorDebugHeapEnum : ICorDebugEnum
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

		// HRESULT Next(
		//     [in] ULONG celt,
		//     [out, size_is(celt), length_is(*pceltFetched)] COR_HEAPOBJECT  objects[],
		//     [out] ULONG *pceltFetched
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		bool Next(
			int celt,
			out COR_HEAPOBJECT values);
	}
}
