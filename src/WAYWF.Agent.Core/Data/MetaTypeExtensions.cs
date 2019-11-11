// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Immutable;
using WAYWF.Agent.Core.CorDebugApi;
using WAYWF.Agent.Data;

namespace WAYWF.Agent.Core
{
	static class MetaTypeExtensions
	{
		public static bool TryGetValue(this MetaTypeBase type, ICorDebugValue value, out object result)
		{
			var tmp = type.Apply(Visitor.Instance, new Context(value, ImmutableArray<MetaTypeBase>.Empty));

			if (tmp == UnknownValue)
			{
				result = null;
				return false;
			}

			result = tmp;
			return true;
		}

		static readonly object UnknownValue = new object();

		sealed class Visitor : IMetaTypeVisitor<Context, object>
		{
			public static Visitor Instance { get; } = new Visitor();

			public object VisitArray(MetaArrayType metaType, Context context)
			{
				if (context.Value is ICorDebugArrayValue arr)
				{
					var rank = arr.GetRank();
					var dims = new int[rank];
					arr.GetDimensions(rank, dims);

					var formatter = new MetaFormatter();
					formatter.Write(metaType, dims);

					return formatter.ToString();
				}

				return UnknownValue;
			}

			public object VisitEnum(MetaEnumType metaType, Context context)
			{
				var tmp = metaType.UnderlyingType.Apply(this, context);

				if (tmp == UnknownValue)
				{
					return UnknownValue;
				}

				return metaType.Format(ULongFromObject(tmp));
			}

			public object VisitGCHandle(MetaGCHandleType metaType, Context context)
			{
				var objValue = (ICorDebugObjectValue)context.Value;
				var handleObj = objValue.GetFieldValue(metaType.HandleField);

				if (handleObj == null)
				{
					return UnknownValue;
				}

				return ValueExtensions.GetValue<IntPtr>((ICorDebugGenericValue)handleObj).ToString();
			}

			public object VisitGen(MetaGenType metaType, Context context)
				=> metaType.BaseType.Apply(this, new Context(context.Value, metaType.TypeArgs));

			public object VisitKnownType(MetaKnownType metaType, Context context)
			{
				var value = context.Value;

				return metaType.Code switch
				{
					MetaKnownTypeCode.String => GetStringValue(value),
					MetaKnownTypeCode.Boolean => GetGenericValue<bool>(value),
					MetaKnownTypeCode.Char => GetGenericValue<char>(value),
					MetaKnownTypeCode.SByte => GetGenericValue<sbyte>(value),
					MetaKnownTypeCode.Int16 => GetGenericValue<short>(value),
					MetaKnownTypeCode.Int32 => GetGenericValue<int>(value),
					MetaKnownTypeCode.Int64 => GetGenericValue<long>(value),
					MetaKnownTypeCode.IntPtr => GetGenericValue<IntPtr>(value),
					MetaKnownTypeCode.Byte => GetGenericValue<byte>(value),
					MetaKnownTypeCode.UInt16 => GetGenericValue<ushort>(value),
					MetaKnownTypeCode.UInt32 => GetGenericValue<uint>(value),
					MetaKnownTypeCode.UInt64 => GetGenericValue<ulong>(value),
					MetaKnownTypeCode.UIntPtr => GetGenericValue<UIntPtr>(value),
					MetaKnownTypeCode.Single => GetGenericValue<float>(value),
					MetaKnownTypeCode.Double => GetGenericValue<double>(value),
					MetaKnownTypeCode.Decimal => GetGenericValue<decimal>(value),
					_ => UnknownValue,
				};
			}

			public object VisitNullable(MetaNullableType metaType, Context context)
			{
				var typeArgs = context.TypeArgs;

				if (typeArgs.Length != 1)
				{
					throw new InvalidMetaDataException("Nullable should have exactly 1 type arg");
				}

				var innerType = typeArgs[0];
				var objValue = (ICorDebugObjectValue)context.Value;
				var hasValueValue = objValue.GetFieldValue(metaType.HasValueToken);

				if (!ValueExtensions.GetValue<bool>((ICorDebugGenericValue)hasValueValue))
				{
					return UnknownValue;
				}

				var valueValue = objValue.GetFieldValue(metaType.ValueToken);

				if (valueValue == null)
				{
					return UnknownValue;
				}

				return innerType.Apply(this, new Context(valueValue, ImmutableArray<MetaTypeBase>.Empty));
			}

			public object VisitPointer(MetaPointerType metaType, Context context) => UnknownValue;
			public object VisitSimpleResolved(MetaSimpleResolvedType metaType, Context context) => UnknownValue;
			public object VisitUnresolved(MetaUnresolvedType metaType, Context context) => UnknownValue;
			public object VisitVar(MetaVarType metaType, Context context) => UnknownValue;

			static object GetStringValue(ICorDebugValue value)
			{
				if (value is ICorDebugStringValue sValue)
				{
					return sValue.GetString();
				}

				return UnknownValue;
			}

			static object GetGenericValue<T>(ICorDebugValue value)
				where T : unmanaged
			{
				if (value is ICorDebugGenericValue gValue)
				{
					return ValueExtensions.GetValue<T>(gValue);
				}

				return UnknownValue;
			}

			static ulong ULongFromObject(object obj)
			{
				unchecked
				{
					return (Type.GetTypeCode(obj.GetType())) switch
					{
						TypeCode.Boolean => ((bool)obj) ? 1u : 0u,
						TypeCode.Char => (char)obj,

						TypeCode.SByte => (byte)(sbyte)obj,
						TypeCode.Int16 => (ushort)(short)obj,
						TypeCode.Int32 => (uint)(int)obj,
						TypeCode.Int64 => (ulong)(long)obj,

						TypeCode.Byte => (byte)obj,
						TypeCode.UInt16 => (ushort)obj,
						TypeCode.UInt32 => (uint)obj,
						TypeCode.UInt64 => (ulong)obj,

						_ => obj switch
						{
							IntPtr intPtr => (ulong)intPtr,
							UIntPtr uIntPtr => (ulong)uIntPtr,
							_ => throw new ArgumentException("cannot convert " + obj.GetType() + " to ulong"),
						},
					};
				}
			}
		}

		struct Context
		{
			public Context(ICorDebugValue value, ImmutableArray<MetaTypeBase> typeArgs)
			{
				Value = value;
				TypeArgs = typeArgs;
			}

			public ICorDebugValue Value { get; }
			public ImmutableArray<MetaTypeBase> TypeArgs { get; }
		}
	}
}
