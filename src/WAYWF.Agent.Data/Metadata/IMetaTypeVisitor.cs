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
}
