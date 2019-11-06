// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Generic;
using System.Text;
using WAYWF.Agent.Data;

namespace WAYWF.Agent
{
	sealed class MetaFormatter : IMetaTypeVisitor
	{
		public IList<MetaTypeBase> TypeArgs { get; set; }
		public int MethodArgsStart { get; set; }

		public override string ToString() => _builder.ToString();
		public void Clear() => _builder.Clear();

		public void Write(MetaMethod method)
		{
			_builder.Append(method.Name);

			WriteTypeArgs(TypeArgs, MethodArgsStart, method.Signature.TypeArg);
		}

		public void Write(MetaTypeBase type)
		{
			type.Apply(this);
		}

		public void Write(MetaResolvedType type, IList<MetaTypeBase> overrideTypeArgs)
		{
			int typeArgStart;

			if (type.DeclaringType != null)
			{
				typeArgStart = type.DeclaringType.TypeArgs;
				Write(type.DeclaringType, overrideTypeArgs);
				_builder.Append('.');
			}
			else
			{
				typeArgStart = 0;
			}

			var typeArgCount = type.TypeArgs - typeArgStart;

			if (typeArgCount == 0 || overrideTypeArgs == null)
			{
				_builder.Append(type.Name);
			}
			else
			{
				AppendGenericTypeName(type.Name);
				WriteTypeArgs(overrideTypeArgs, typeArgStart, typeArgCount);
			}
		}

		public void Write(MetaUnresolvedType type, IList<MetaTypeBase> overrideTypeArgs)
		{
			if (type.DeclaringType != null)
			{
				Write(type.DeclaringType, null);
				_builder.Append('.');
			}

			_builder.Append(type.Name);

			if (overrideTypeArgs != null)
			{
				WriteTypeArgs(overrideTypeArgs, 0, overrideTypeArgs.Count);
			}
		}

		public void Write(MetaArrayType type, int[] dimensions)
		{
			var current = GetElementType(type.ElementType);

			current.Apply(this);

			_builder.Append('[');
			_builder.Append(dimensions[0]);

			for (var i = 1; i < type.Rank; i++)
			{
				_builder.Append(", ");
				_builder.Append(dimensions[i]);
			}

			_builder.Append(']');

			type = type.ElementType as MetaArrayType;

			WriteIndexers(type);
		}

		void AppendGenericTypeName(string name)
		{
			var tildIndex = name.IndexOf('`');

			if (tildIndex < 0)
			{
				_builder.Append(name);
			}
			else
			{
				_builder.Append(name, 0, tildIndex);
			}
		}

		void WriteTypeArgs(IList<MetaTypeBase> typeArgs, int start, int count)
		{
			var end = start + count;

			if (start < end)
			{
				_builder.Append('<');

				typeArgs[start++].Apply(this);

				while (start < end)
				{
					_builder.Append(", ");
					typeArgs[start++].Apply(this);
				}

				_builder.Append('>');
			}
		}

		static MetaTypeBase GetElementType(MetaTypeBase type)
		{
			while (type is MetaArrayType tmp)
			{
				type = tmp.ElementType;
			}

			return type;
		}

		MetaArrayType WriteIndexers(MetaArrayType type)
		{
			for (; type != null; type = type.ElementType as MetaArrayType)
			{
				_builder.Append('[');
				_builder.Append(',', type.Rank - 1);
				_builder.Append(']');
			}
			return type;
		}

		#region IMetaTypeVisitor Members

		void IMetaTypeVisitor.VisitArray(MetaArrayType metaType)
		{
			var current = GetElementType(metaType.ElementType);

			current.Apply(this);
			metaType = WriteIndexers(metaType);
		}

		void IMetaTypeVisitor.VisitPointer(MetaPointerType metaType)
		{
			metaType.ElementType.Apply(this);
			_builder.Append('*');
		}

		void IMetaTypeVisitor.VisitVar(MetaVarType metaType)
		{
			int lowerBound;
			int upperBound;

			if (metaType.Method)
			{
				lowerBound = MethodArgsStart;
				upperBound = TypeArgs.Count;
			}
			else
			{
				lowerBound = 0;
				upperBound = MethodArgsStart;
			}

			var index = metaType.Index + lowerBound;

			if (index < lowerBound || index >= upperBound)
			{
				throw new ShitFanContactException("Referenced a non-existent type arg.");
			}

			TypeArgs[index].Apply(this);
		}

		void IMetaTypeVisitor.VisitGen(MetaGenType metaType)
		{
			if (metaType.BaseType is MetaResolvedType resolvedType)
			{
				Write(resolvedType, metaType.TypeArgs);
			}
			else
			{
				Write((MetaUnresolvedType)metaType.BaseType, metaType.TypeArgs);
			}
		}

		void IMetaTypeVisitor.VisitEnum(MetaEnumType metaType) => Write(metaType, null);
		void IMetaTypeVisitor.VisitGCHandle(MetaGCHandleType metaType) => Write(metaType, null);
		void IMetaTypeVisitor.VisitKnownType(MetaKnownType metaType) => Write(metaType, null);
		void IMetaTypeVisitor.VisitNullable(MetaNullableType metaType) => Write(metaType, null);
		void IMetaTypeVisitor.VisitSimpleResolved(MetaSimpleResolvedType metaType) => Write(metaType, null);

		void IMetaTypeVisitor.VisitUnresolved(MetaUnresolvedType metaType)
		{
			Write(metaType, null);
		}

		#endregion

		readonly StringBuilder _builder = new StringBuilder();
	}
}
