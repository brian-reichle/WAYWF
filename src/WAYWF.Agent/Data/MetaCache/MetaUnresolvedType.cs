// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;

namespace WAYWF.Agent.MetaCache
{
	[DebuggerDisplay("Type: {Name,nq}")]
	sealed class MetaUnresolvedType : MetaType
	{
		public MetaUnresolvedType(MetaDataToken token, MetaUnresolvedType declaringType, string name)
			: base(token, name)
		{
			DeclaringType = declaringType;
		}

		public MetaUnresolvedType DeclaringType { get; }

		public override void Apply(IMetaTypeVisitor visitor) => visitor.VisitUnresolved(this);
		public override TResult Apply<TArg, TResult>(IMetaTypeVisitor<TArg, TResult> visitor, TArg arg) => visitor.VisitUnresolved(this, arg);
	}
}
