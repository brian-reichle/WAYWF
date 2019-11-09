// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.Core.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("4A2A1EC9-85EC-4BFB-9F15-A89FDFE0FE83")]
	interface ICorDebugAssemblyEnum : ICorDebugEnum
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
		//     [out, size_is(celt), length_is(*pceltFetched)] ICorDebugAssembly *values[],
		//     [out] ULONG *pceltFetched
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		bool Next(
			int celt,
			[MarshalAs(UnmanagedType.Interface)] out ICorDebugAssembly values);
	}
}
