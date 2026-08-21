// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Reflection;

namespace WAYWF.Agent.Data.Test;

abstract class DummyBaseMetaTypeVisitor<TArg, TResult> : IMetaTypeVisitor<TArg, TResult>
{
	protected DummyBaseMetaTypeVisitor()
	{
	}

	public virtual TResult VisitArray(MetaArrayType metaType, TArg arg) => Visit(MethodBase.GetCurrentMethod(), metaType, arg);
	public virtual TResult VisitEnum(MetaEnumType metaType, TArg arg) => Visit(MethodBase.GetCurrentMethod(), metaType, arg);
	public virtual TResult VisitGCHandle(MetaGCHandleType metaType, TArg arg) => Visit(MethodBase.GetCurrentMethod(), metaType, arg);
	public virtual TResult VisitGen(MetaGenType metaType, TArg arg) => Visit(MethodBase.GetCurrentMethod(), metaType, arg);
	public virtual TResult VisitKnownType(MetaKnownType metaType, TArg arg) => Visit(MethodBase.GetCurrentMethod(), metaType, arg);
	public virtual TResult VisitNullable(MetaNullableType metaType, TArg arg) => Visit(MethodBase.GetCurrentMethod(), metaType, arg);
	public virtual TResult VisitPointer(MetaPointerType metaType, TArg arg) => Visit(MethodBase.GetCurrentMethod(), metaType, arg);
	public virtual TResult VisitSimpleResolved(MetaSimpleResolvedType metaType, TArg arg) => Visit(MethodBase.GetCurrentMethod(), metaType, arg);
	public virtual TResult VisitUnresolved(MetaUnresolvedType metaType, TArg arg) => Visit(MethodBase.GetCurrentMethod(), metaType, arg);
	public virtual TResult VisitVar(MetaVarType metaType, TArg arg) => Visit(MethodBase.GetCurrentMethod(), metaType, arg);

	protected virtual TResult Visit(MethodBase method, MetaTypeBase type, TArg arg) => throw new InvalidOperationException($"Did not expect '{method.Name}' to be accessed.");
}
