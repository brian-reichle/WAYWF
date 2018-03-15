// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace WAYWF.Agent.CorDebugApi
{
	[StructLayout(LayoutKind.Sequential)]
	struct COR_TYPEID : IEquatable<COR_TYPEID>
	{
		// UINT64 token1;
		long _token1;

		// UINT64 token2;
		long _token2;

		public bool Equals(COR_TYPEID other) => _token1 == other._token1 && _token2 == other._token2;
		public override bool Equals(object obj) => obj is COR_TYPEID && Equals((COR_TYPEID)obj);
		public override int GetHashCode() => _token1.GetHashCode() ^ _token2.GetHashCode();
	}
}
