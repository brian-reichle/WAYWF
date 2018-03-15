// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace WAYWF.Agent.MetaCache
{
	interface IMetaTypeVisitor
	{
		void Visit(MetaArrayType metaArrayType);
		void Visit(MetaPointerType metaPointerType);
		void Visit(MetaVarType metaVarType);
		void Visit(MetaGenType metaGenType);
		void Visit(MetaResolvedType metaType);
		void Visit(MetaUnresolvedType metaUnresolvedType);
	}
}
