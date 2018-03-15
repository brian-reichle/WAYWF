// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using WAYWF.Agent.CorDebugApi;

namespace WAYWF.Agent
{
	partial class ValueExtensions
	{
		public static unsafe bool GetBoolean(this ICorDebugGenericValue value)
		{
			if (value.GetSize() != sizeof(bool)) throw new ArgumentException("size mismatch");

			bool tmp;
			value.GetValue((IntPtr)(&tmp));
			return tmp;
		}
		public static unsafe char GetChar(this ICorDebugGenericValue value)
		{
			if (value.GetSize() != sizeof(char)) throw new ArgumentException("size mismatch");

			char tmp;
			value.GetValue((IntPtr)(&tmp));
			return tmp;
		}
		public static unsafe sbyte GetSByte(this ICorDebugGenericValue value)
		{
			if (value.GetSize() != sizeof(sbyte)) throw new ArgumentException("size mismatch");

			sbyte tmp;
			value.GetValue((IntPtr)(&tmp));
			return tmp;
		}
		public static unsafe short GetInt16(this ICorDebugGenericValue value)
		{
			if (value.GetSize() != sizeof(short)) throw new ArgumentException("size mismatch");

			short tmp;
			value.GetValue((IntPtr)(&tmp));
			return tmp;
		}
		public static unsafe int GetInt32(this ICorDebugGenericValue value)
		{
			if (value.GetSize() != sizeof(int)) throw new ArgumentException("size mismatch");

			int tmp;
			value.GetValue((IntPtr)(&tmp));
			return tmp;
		}
		public static unsafe long GetInt64(this ICorDebugGenericValue value)
		{
			if (value.GetSize() != sizeof(long)) throw new ArgumentException("size mismatch");

			long tmp;
			value.GetValue((IntPtr)(&tmp));
			return tmp;
		}
		public static unsafe IntPtr GetIntPtr(this ICorDebugGenericValue value)
		{
			if (value.GetSize() != sizeof(IntPtr)) throw new ArgumentException("size mismatch");

			IntPtr tmp;
			value.GetValue((IntPtr)(&tmp));
			return tmp;
		}
		public static unsafe byte GetByte(this ICorDebugGenericValue value)
		{
			if (value.GetSize() != sizeof(byte)) throw new ArgumentException("size mismatch");

			byte tmp;
			value.GetValue((IntPtr)(&tmp));
			return tmp;
		}
		public static unsafe ushort GetUInt16(this ICorDebugGenericValue value)
		{
			if (value.GetSize() != sizeof(ushort)) throw new ArgumentException("size mismatch");

			ushort tmp;
			value.GetValue((IntPtr)(&tmp));
			return tmp;
		}
		public static unsafe uint GetUInt32(this ICorDebugGenericValue value)
		{
			if (value.GetSize() != sizeof(uint)) throw new ArgumentException("size mismatch");

			uint tmp;
			value.GetValue((IntPtr)(&tmp));
			return tmp;
		}
		public static unsafe ulong GetUInt64(this ICorDebugGenericValue value)
		{
			if (value.GetSize() != sizeof(ulong)) throw new ArgumentException("size mismatch");

			ulong tmp;
			value.GetValue((IntPtr)(&tmp));
			return tmp;
		}
		public static unsafe UIntPtr GetUIntPtr(this ICorDebugGenericValue value)
		{
			if (value.GetSize() != sizeof(UIntPtr)) throw new ArgumentException("size mismatch");

			UIntPtr tmp;
			value.GetValue((IntPtr)(&tmp));
			return tmp;
		}
		public static unsafe float GetSingle(this ICorDebugGenericValue value)
		{
			if (value.GetSize() != sizeof(float)) throw new ArgumentException("size mismatch");

			float tmp;
			value.GetValue((IntPtr)(&tmp));
			return tmp;
		}
		public static unsafe double GetDouble(this ICorDebugGenericValue value)
		{
			if (value.GetSize() != sizeof(double)) throw new ArgumentException("size mismatch");

			double tmp;
			value.GetValue((IntPtr)(&tmp));
			return tmp;
		}
		public static unsafe decimal GetDecimal(this ICorDebugGenericValue value)
		{
			if (value.GetSize() != sizeof(decimal)) throw new ArgumentException("size mismatch");

			decimal tmp;
			value.GetValue((IntPtr)(&tmp));
			return tmp;
		}
		public static unsafe Guid GetGuid(this ICorDebugGenericValue value)
		{
			if (value.GetSize() != sizeof(Guid)) throw new ArgumentException("size mismatch");

			Guid tmp;
			value.GetValue((IntPtr)(&tmp));
			return tmp;
		}
	}
}
