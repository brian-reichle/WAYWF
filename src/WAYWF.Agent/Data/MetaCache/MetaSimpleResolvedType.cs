// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace WAYWF.Agent.MetaCache
{
	sealed class MetaSimpleResolvedType : MetaResolvedType
	{
		public MetaSimpleResolvedType(MetaModule module, MetaDataToken token, MetaResolvedType declaringType, string name, int typeArgs)
			: base(module, token, declaringType, name, typeArgs)
		{
		}

		public override void Apply(IMetaTypeVisitor visitor) => visitor.VisitSimpleResolved(this);
		public override TResult Apply<TArg, TResult>(IMetaTypeVisitor<TArg, TResult> visitor, TArg arg) => visitor.VisitSimpleResolved(this, arg);
	}
}
