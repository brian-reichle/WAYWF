// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Reflection;

namespace WAYWF.Agent.Data.Test
{
	abstract class DummyBaseMetaTypeVisitor : IMetaTypeVisitor
	{
		protected DummyBaseMetaTypeVisitor()
		{
		}

		public virtual void VisitArray(MetaArrayType metaType) => Visit(MethodBase.GetCurrentMethod(), metaType);
		public virtual void VisitEnum(MetaEnumType metaType) => Visit(MethodBase.GetCurrentMethod(), metaType);
		public virtual void VisitGCHandle(MetaGCHandleType metaType) => Visit(MethodBase.GetCurrentMethod(), metaType);
		public virtual void VisitGen(MetaGenType metaType) => Visit(MethodBase.GetCurrentMethod(), metaType);
		public virtual void VisitKnownType(MetaKnownType metaType) => Visit(MethodBase.GetCurrentMethod(), metaType);
		public virtual void VisitNullable(MetaNullableType metaType) => Visit(MethodBase.GetCurrentMethod(), metaType);
		public virtual void VisitPointer(MetaPointerType metaType) => Visit(MethodBase.GetCurrentMethod(), metaType);
		public virtual void VisitSimpleResolved(MetaSimpleResolvedType metaType) => Visit(MethodBase.GetCurrentMethod(), metaType);
		public virtual void VisitUnresolved(MetaUnresolvedType metaType) => Visit(MethodBase.GetCurrentMethod(), metaType);
		public virtual void VisitVar(MetaVarType metaType) => Visit(MethodBase.GetCurrentMethod(), metaType);

		protected virtual void Visit(MethodBase method, MetaTypeBase type) => throw new InvalidOperationException($"Did not expect '{method.Name}' to be accessed.");
	}
}
