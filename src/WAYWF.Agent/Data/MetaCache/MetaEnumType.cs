// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Globalization;
using System.Text;
using WAYWF.Agent.CorDebugApi;

namespace WAYWF.Agent.MetaCache
{
	sealed class MetaEnumType : MetaResolvedType
	{
		public MetaEnumType(MetaModule module, MetaDataToken token, MetaResolvedType declaringType, string name, MetaKnownType underlyingType, bool isFlags, string[] labels, ulong[] values)
			: base(module, token, declaringType, name, 0)
		{
			if (underlyingType == null) throw new ArgumentNullException(nameof(underlyingType));
			if (labels == null) throw new ArgumentNullException(nameof(labels));
			if (values == null) throw new ArgumentNullException(nameof(values));
			if (labels.Length != values.Length) throw new ArgumentException("length mismatch.");

			UnderlyingType = underlyingType;
			_isFlags = isFlags;
			_labels = labels;
			_values = values;
		}

		public MetaKnownType UnderlyingType { get; }

		public override bool TryGetValue(ICorDebugValue value, out object result)
		{
			if (UnderlyingType.TryGetValue(value, out result))
			{
				result = Format(ULongFromObject(result));
				return true;
			}

			return false;
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
						var tmp = (IntPtr?)obj;
						if (tmp.HasValue) return (ulong)tmp.Value;

						var utmp = (UIntPtr?)obj;
						if (utmp.HasValue) return (ulong)tmp.Value;

						throw new ArgumentException("cannot convert " + obj.GetType() + " to ulong");
				}
			}
		}

		string Format(ulong value)
		{
			return _isFlags ? FormatFlags(value) : FormatEnum(value);
		}

		string FormatEnum(ulong value)
		{
			var index = Array.BinarySearch(_values, value);
			return index < 0 ? value.ToString(CultureInfo.InvariantCulture) : _labels[index];
		}

		string FormatFlags(ulong value)
		{
			if (value == 0)
			{
				return FormatEnum(value);
			}

			string text = null;
			StringBuilder builder = null;

			for (var i = _values.Length - 1; i >= 0; i--)
			{
				var tmp = _values[i];
				if (tmp == 0) continue;

				if ((value & tmp) == tmp)
				{
					var nextText = _labels[i];
					value &= ~tmp;

					if (builder != null)
					{
						builder.Append('|');
						builder.Append(nextText);
					}
					else if (text != null)
					{
						builder = new StringBuilder(text);
						builder.Append('|');
						builder.Append(nextText);
					}
					else
					{
						text = nextText;
					}
				}
			}

			if (value != 0)
			{
				if (builder != null)
				{
					builder.Append('|');
					builder.Append(value);
				}
				else if (text != null)
				{
					builder = new StringBuilder(text);
					builder.Append('|');
					builder.Append(value);
				}
				else
				{
					text = value.ToString(CultureInfo.InvariantCulture);
				}
			}

			if (builder != null)
			{
				return builder.ToString();
			}
			else
			{
				return text;
			}
		}

		readonly bool _isFlags;
		readonly string[] _labels;
		readonly ulong[] _values;
	}
}
