// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Diagnostics;
using System.Globalization;
using WAYWF.Agent.Data;

namespace WAYWF.Agent.Core.CorDebugApi
{
	readonly struct CORDB_ADDRESS : IEquatable<CORDB_ADDRESS>, IComparable<CORDB_ADDRESS>, IComparable
	{
		public CORDB_ADDRESS(long address)
		{
			_address = address;
		}

		public override bool Equals(object obj) => obj is CORDB_ADDRESS && Equals((CORDB_ADDRESS)obj);
		public override int GetHashCode() => _address.GetHashCode();
		public override string ToString() => _address.ToString("X16", CultureInfo.InvariantCulture);

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool IsNull => _address == 0;

		[DebuggerStepThrough]
		public static bool operator ==(CORDB_ADDRESS lhs, CORDB_ADDRESS rhs) => lhs.Equals(rhs);

		[DebuggerStepThrough]
		public static bool operator !=(CORDB_ADDRESS lhs, CORDB_ADDRESS rhs) => !lhs.Equals(rhs);

		public static CORDB_ADDRESS operator +(CORDB_ADDRESS baseAddress, int offset) => new CORDB_ADDRESS(baseAddress._address + offset);
		public static CORDB_ADDRESS operator -(CORDB_ADDRESS baseAddress, int offset) => new CORDB_ADDRESS(baseAddress._address - offset);

		public static implicit operator MemoryAddress(CORDB_ADDRESS address) => new MemoryAddress(address._address);

		public static explicit operator IntPtr(CORDB_ADDRESS address) => (IntPtr)address._address;
		public bool Equals(CORDB_ADDRESS other) => _address == other._address;
		public int CompareTo(CORDB_ADDRESS other) => _address.CompareTo(other._address);

		#region IComparable Members

		int IComparable.CompareTo(object obj) => CompareTo((CORDB_ADDRESS)obj);

		#endregion

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly long _address;
	}
}
