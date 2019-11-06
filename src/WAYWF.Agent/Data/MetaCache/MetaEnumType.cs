// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Globalization;
using System.Text;

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

		public override void Apply(IMetaTypeVisitor visitor) => visitor.VisitEnum(this);
		public override TResult Apply<TArg, TResult>(IMetaTypeVisitor<TArg, TResult> visitor, TArg arg) => visitor.VisitEnum(this, arg);

		public string Format(ulong value)
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
