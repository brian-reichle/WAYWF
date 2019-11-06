// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Diagnostics;
using System.Globalization;

namespace WAYWF.Agent
{
	public readonly struct MemoryAddress : IEquatable<MemoryAddress>
	{
		public MemoryAddress(long address)
		{
			_address = address;
		}

		public override bool Equals(object obj) => obj is MemoryAddress address && Equals(address);
		public override int GetHashCode() => _address.GetHashCode();
		public override string ToString() => _address.ToString("X16", CultureInfo.InvariantCulture);

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool IsNull => _address == 0;

		[DebuggerStepThrough]
		public static bool operator ==(MemoryAddress lhs, MemoryAddress rhs) => lhs.Equals(rhs);

		[DebuggerStepThrough]
		public static bool operator !=(MemoryAddress lhs, MemoryAddress rhs) => !lhs.Equals(rhs);

		public bool Equals(MemoryAddress other) => _address == other._address;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly long _address;
	}
}
