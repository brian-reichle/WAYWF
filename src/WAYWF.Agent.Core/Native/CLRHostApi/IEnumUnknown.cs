// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.Core.CLRHostApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("00000100-0000-0000-C000-000000000046")]
	interface IEnumUnknown
	{
		// HRESULT Next(
		//     [in] LONG celt,
		//     [out] IUnknown **rgelt,
		//     [out] ULONG *pceltFetched
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		bool Next(
			int celt,
			[MarshalAs(UnmanagedType.IUnknown)] out object rgelt);

		// HRESULT Skip(
		//     [in] ULONG celt
		// );
		void Skip(
			int celt);

		// HRESULT Reset();
		void Reset();

		// HRESULT Clone(
		//     [out] IEnumUnknown **ppenum
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		IEnumUnknown Clone();
	}
}
