// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;

namespace WAYWF.Agent.Data
{
	public unsafe partial class MetaKnownType
	{
		public static readonly MetaKnownType Boolean = new MetaKnownType(MetaKnownTypeCode.Boolean, "System.Boolean", sizeof(bool));
		public static readonly MetaKnownType Char = new MetaKnownType(MetaKnownTypeCode.Char, "System.Char", sizeof(char));
		public static readonly MetaKnownType SByte = new MetaKnownType(MetaKnownTypeCode.SByte, "System.SByte", sizeof(sbyte));
		public static readonly MetaKnownType Int16 = new MetaKnownType(MetaKnownTypeCode.Int16, "System.Int16", sizeof(short));
		public static readonly MetaKnownType Int32 = new MetaKnownType(MetaKnownTypeCode.Int32, "System.Int32", sizeof(int));
		public static readonly MetaKnownType Int64 = new MetaKnownType(MetaKnownTypeCode.Int64, "System.Int64", sizeof(long));
		public static readonly MetaKnownType IntPtr = new MetaKnownType(MetaKnownTypeCode.IntPtr, "System.IntPtr", sizeof(IntPtr));
		public static readonly MetaKnownType Byte = new MetaKnownType(MetaKnownTypeCode.Byte, "System.Byte", sizeof(byte));
		public static readonly MetaKnownType UInt16 = new MetaKnownType(MetaKnownTypeCode.UInt16, "System.UInt16", sizeof(ushort));
		public static readonly MetaKnownType UInt32 = new MetaKnownType(MetaKnownTypeCode.UInt32, "System.UInt32", sizeof(uint));
		public static readonly MetaKnownType UInt64 = new MetaKnownType(MetaKnownTypeCode.UInt64, "System.UInt64", sizeof(ulong));
		public static readonly MetaKnownType UIntPtr = new MetaKnownType(MetaKnownTypeCode.UIntPtr, "System.UIntPtr", sizeof(UIntPtr));
		public static readonly MetaKnownType Single = new MetaKnownType(MetaKnownTypeCode.Single, "System.Single", sizeof(float));
		public static readonly MetaKnownType Double = new MetaKnownType(MetaKnownTypeCode.Double, "System.Double", sizeof(double));
		public static readonly MetaKnownType Decimal = new MetaKnownType(MetaKnownTypeCode.Decimal, "System.Decimal", sizeof(decimal));
		public static readonly MetaKnownType Guid = new MetaKnownType(MetaKnownTypeCode.Guid, "System.Guid", sizeof(Guid));
		public static readonly MetaKnownType Void = new MetaKnownType(MetaKnownTypeCode.Void, "System.Void", 0);
		public static readonly MetaKnownType Object = new MetaKnownType(MetaKnownTypeCode.Object, "System.Object", 0);
		public static readonly MetaKnownType TypedReference = new MetaKnownType(MetaKnownTypeCode.TypedReference, "System.TypedReference", 0);
		public static readonly MetaKnownType String = new MetaKnownType(MetaKnownTypeCode.String, "System.String", 0);

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
