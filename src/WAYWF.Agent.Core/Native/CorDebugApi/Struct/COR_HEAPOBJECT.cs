// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Runtime.InteropServices;

namespace WAYWF.Agent.Core.CorDebugApi
{
	[StructLayout(LayoutKind.Sequential)]
	struct COR_HEAPOBJECT
	{
		// CORDB_ADDRESS address;
		public CORDB_ADDRESS address;

		// ULONG64 size;
		public long size;

		// COR_TYPEID type;
		public COR_TYPEID type;
	}
}
