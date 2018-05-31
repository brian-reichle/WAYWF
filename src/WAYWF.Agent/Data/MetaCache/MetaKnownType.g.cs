// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using WAYWF.Agent.CorDebugApi;

namespace WAYWF.Agent.MetaCache
{
	internal unsafe partial class MetaKnownType
	{
		public static readonly MetaKnownType String = new MetaStringType();
		public static readonly MetaKnownType Boolean = new MetaValueType<bool>();
		public static readonly MetaKnownType Char = new MetaValueType<char>();
		public static readonly MetaKnownType SByte = new MetaValueType<sbyte>();
		public static readonly MetaKnownType Int16 = new MetaValueType<short>();
		public static readonly MetaKnownType Int32 = new MetaValueType<int>();
		public static readonly MetaKnownType Int64 = new MetaValueType<long>();
		public static readonly MetaKnownType IntPtr = new MetaValueType<IntPtr>();
		public static readonly MetaKnownType Byte = new MetaValueType<byte>();
		public static readonly MetaKnownType UInt16 = new MetaValueType<ushort>();
		public static readonly MetaKnownType UInt32 = new MetaValueType<uint>();
		public static readonly MetaKnownType UInt64 = new MetaValueType<ulong>();
		public static readonly MetaKnownType UIntPtr = new MetaValueType<UIntPtr>();
		public static readonly MetaKnownType Single = new MetaValueType<float>();
		public static readonly MetaKnownType Double = new MetaValueType<double>();
		public static readonly MetaKnownType Decimal = new MetaValueType<decimal>();
		public static readonly MetaKnownType Guid = new MetaValueType<Guid>();
		public static readonly MetaKnownType Void = new MetaKnownType("System.Void", 0);
		public static readonly MetaKnownType Object = new MetaKnownType("System.Object", 0);
		public static readonly MetaKnownType TypedReference = new MetaKnownType("System.TypedReference", 0);

		public static MetaKnownType FromElementType(CorElementType type)
		{
			switch (type)
			{
				case CorElementType.ELEMENT_TYPE_BOOLEAN: return Boolean;
				case CorElementType.ELEMENT_TYPE_CHAR: return Char;
				case CorElementType.ELEMENT_TYPE_I: return IntPtr;
				case CorElementType.ELEMENT_TYPE_I1: return SByte;
				case CorElementType.ELEMENT_TYPE_I2: return Int16;
				case CorElementType.ELEMENT_TYPE_I4: return Int32;
				case CorElementType.ELEMENT_TYPE_I8: return Int64;
				case CorElementType.ELEMENT_TYPE_OBJECT: return Object;
				case CorElementType.ELEMENT_TYPE_R4: return Single;
				case CorElementType.ELEMENT_TYPE_R8: return Double;
				case CorElementType.ELEMENT_TYPE_STRING: return String;
				case CorElementType.ELEMENT_TYPE_TYPEDBYREF: return TypedReference;
				case CorElementType.ELEMENT_TYPE_U: return UIntPtr;
				case CorElementType.ELEMENT_TYPE_U1: return Byte;
				case CorElementType.ELEMENT_TYPE_U2: return UInt16;
				case CorElementType.ELEMENT_TYPE_U4: return UInt32;
				case CorElementType.ELEMENT_TYPE_U8: return UInt64;
				case CorElementType.ELEMENT_TYPE_VOID: return Void;
				default: throw new ResolutionException(type);
			}
		}

		static readonly Dictionary<string, MetaKnownType> _lookup = new Dictionary<string, MetaKnownType>()
		{
			{ Boolean.Name, Boolean },
			{ Char.Name, Char },
			{ SByte.Name, SByte },
			{ Int16.Name, Int16 },
			{ Int32.Name, Int32 },
			{ Int64.Name, Int64 },
			{ IntPtr.Name, IntPtr },
			{ Byte.Name, Byte },
			{ UInt16.Name, UInt16 },
			{ UInt32.Name, UInt32 },
			{ UInt64.Name, UInt64 },
			{ UIntPtr.Name, UIntPtr },
			{ Single.Name, Single },
			{ Double.Name, Double },
			{ Decimal.Name, Decimal },
			{ Guid.Name, Guid },
			{ Void.Name, Void },
			{ Object.Name, Object },
			{ TypedReference.Name, TypedReference },
			{ String.Name, String },
		};
	}
}
