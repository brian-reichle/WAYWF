// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.ObjectModel;
using WAYWF.Agent.CorDebugApi;
using WAYWF.Agent.Data;

namespace WAYWF.Agent.MetaCache
{
	static class MetaTypeExtensions
	{
		public static bool TryGetValue(this MetaTypeBase type, ICorDebugValue value, out object result)
		{
			var tmp = type.Apply(Visitor.Instance, new Context(value, null));

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

				switch (metaType.Code)
				{
					case MetaKnownTypeCode.String: return GetStringValue(value);
					case MetaKnownTypeCode.Boolean: return GetGenericValue<bool>(value);
					case MetaKnownTypeCode.Char: return GetGenericValue<char>(value);
					case MetaKnownTypeCode.SByte: return GetGenericValue<sbyte>(value);
					case MetaKnownTypeCode.Int16: return GetGenericValue<short>(value);
					case MetaKnownTypeCode.Int32: return GetGenericValue<int>(value);
					case MetaKnownTypeCode.Int64: return GetGenericValue<long>(value);
					case MetaKnownTypeCode.IntPtr: return GetGenericValue<IntPtr>(value);
					case MetaKnownTypeCode.Byte: return GetGenericValue<byte>(value);
					case MetaKnownTypeCode.UInt16: return GetGenericValue<ushort>(value);
					case MetaKnownTypeCode.UInt32: return GetGenericValue<uint>(value);
					case MetaKnownTypeCode.UInt64: return GetGenericValue<ulong>(value);
					case MetaKnownTypeCode.UIntPtr: return GetGenericValue<UIntPtr>(value);
					case MetaKnownTypeCode.Single: return GetGenericValue<float>(value);
					case MetaKnownTypeCode.Double: return GetGenericValue<double>(value);
					case MetaKnownTypeCode.Decimal: return GetGenericValue<decimal>(value);
				}

				return UnknownValue;
			}

			public object VisitNullable(MetaNullableType metaType, Context context)
			{
				var typeArgs = context.TypeArgs;

				if (typeArgs.Count != 1)
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

				return innerType.Apply(this, new Context(valueValue, null));
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
					switch (Type.GetTypeCode(obj.GetType()))
					{
						case TypeCode.Boolean: return ((bool)obj) ? 1u : 0u;
						case TypeCode.Char: return (char)obj;

						case TypeCode.SByte: return (byte)(sbyte)obj;
						case TypeCode.Int16: return (ushort)(short)obj;
						case TypeCode.Int32: return (uint)(int)obj;
						case TypeCode.Int64: return (ulong)(long)obj;

						case TypeCode.Byte: return (byte)obj;
						case TypeCode.UInt16: return (ushort)obj;
						case TypeCode.UInt32: return (uint)obj;
						case TypeCode.UInt64: return (ulong)obj;

						default:
							switch (obj)
							{
								case IntPtr intPtr: return (ulong)intPtr;
								case UIntPtr uIntPtr: return (ulong)uIntPtr;
							}

							throw new ArgumentException("cannot convert " + obj.GetType() + " to ulong");
					}
				}
			}
		}

		struct Context
		{
			public Context(ICorDebugValue value, ReadOnlyCollection<MetaTypeBase> typeArgs)
			{
				Value = value;
				TypeArgs = typeArgs;
			}

			public ICorDebugValue Value { get; }
			public ReadOnlyCollection<MetaTypeBase> TypeArgs { get; }
		}
	}
}
