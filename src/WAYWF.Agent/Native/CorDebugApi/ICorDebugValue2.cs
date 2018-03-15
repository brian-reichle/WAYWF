// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("5E0B54E7-D88A-4626-9420-A691E0A78B49")]
	interface ICorDebugValue2
	{
		// HRESULT GetExactType(
		//     [out] ICorDebugType   **ppType
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugType GetExactType();
	}
}
