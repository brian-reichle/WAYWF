// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using WAYWF.Agent.CorDebugApi;

namespace WAYWF.Agent.MetaCache
{
	partial class MetaKnownType : MetaResolvedType
	{
		protected MetaKnownType(string fullName, int size)
			: base(null, MetaDataToken.Nil, null, fullName, 0)
		{
			Size = size;
		}

		public static MetaKnownType FromFullName(string fullName)
		{
			_lookup.TryGetValue(fullName, out var result);
			return result;
		}

		public int Size { get; }

		public bool CanGetEnumValue
		{
			get
			{
				switch (Size)
				{
					case 1:
					case 2:
					case 4:
					case 8:
						return true;
				}

				return false;
			}
		}

		public unsafe ulong GetEnumValue(IntPtr valuePtr)
		{
			if (valuePtr == System.IntPtr.Zero) throw new ArgumentNullException(nameof(valuePtr));

			ulong value;

			switch (Size)
			{
				case 1: value = *(byte*)valuePtr; break;
				case 2: value = *(ushort*)valuePtr; break;
				case 4: value = *(uint*)valuePtr; break;
				case 8: value = *(ulong*)valuePtr; break;
				default: throw new InvalidOperationException("Type has no valid size information.");
			}

			return value;
		}

		sealed class MetaStringType : MetaKnownType
		{
			public MetaStringType()
				: base("System.String", 0)
			{
			}

			public override bool TryGetValue(ICorDebugValue value, out object result)
			{
				if (value is ICorDebugStringValue sValue)
				{
					result = sValue.GetString();
					return true;
				}

				result = null;
				return false;
			}
		}

		sealed class MetaValueType<T> : MetaKnownType
			where T : unmanaged
		{
			public unsafe MetaValueType()
				: base(typeof(T).FullName, sizeof(T))
			{
			}

			public override bool TryGetValue(ICorDebugValue value, out object result)
			{
				if (value is ICorDebugGenericValue gValue)
				{
					result = ValueExtensions.GetValue<T>(gValue);
					return true;
				}

				result = null;
				return false;
			}
		}
	}
}
