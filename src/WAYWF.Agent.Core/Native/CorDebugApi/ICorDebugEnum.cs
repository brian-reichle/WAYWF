// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.Core.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("CC7BCB01-8A68-11D2-983C-0000F808342D")]
	interface ICorDebugEnum
	{
		// HRESULT Skip(
		//     [in] ULONG celt
		// );
		void Skip(
			int celt);

		// HRESULT Reset();
		void Reset();

		// HRESULT Clone(
		//     [out] ICorDebugEnum **ppEnum
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugEnum Clone();

		// HRESULT GetCount(
		//     [out] ULONG *pcelt
		// );
		int GetCount();
	}
}
