// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace WAYWF.Agent.Data
{
	public interface IMetaTypeVisitor
	{
		void VisitArray(MetaArrayType metaType);
		void VisitEnum(MetaEnumType metaType);
		void VisitGCHandle(MetaGCHandleType metaType);
		void VisitGen(MetaGenType metaType);
		void VisitKnownType(MetaKnownType metaType);
		void VisitNullable(MetaNullableType metaType);
		void VisitPointer(MetaPointerType metaType);
		void VisitSimpleResolved(MetaSimpleResolvedType metaType);
		void VisitUnresolved(MetaUnresolvedType metaType);
		void VisitVar(MetaVarType metaType);
	}

	public interface IMetaTypeVisitor<TArg, TResult>
	{
		TResult VisitArray(MetaArrayType metaType, TArg arg);
		TResult VisitEnum(MetaEnumType metaType, TArg arg);
		TResult VisitGCHandle(MetaGCHandleType metaType, TArg arg);
		TResult VisitGen(MetaGenType metaType, TArg arg);
		TResult VisitKnownType(MetaKnownType metaType, TArg arg);
		TResult VisitNullable(MetaNullableType metaType, TArg arg);
		TResult VisitPointer(MetaPointerType metaType, TArg arg);
		TResult VisitSimpleResolved(MetaSimpleResolvedType metaType, TArg arg);
		TResult VisitUnresolved(MetaUnresolvedType metaType, TArg arg);
		TResult VisitVar(MetaVarType metaType, TArg arg);
	}
}
