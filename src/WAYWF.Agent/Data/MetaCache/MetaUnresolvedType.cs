// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.ObjectModel;
using System.Diagnostics;
using WAYWF.Agent.CorDebugApi;

namespace WAYWF.Agent.MetaCache
{
	[DebuggerDisplay("Type: {Name,nq}")]
	class MetaUnresolvedType : MetaType
	{
		public MetaUnresolvedType(MetaDataToken token, MetaUnresolvedType declaringType, string name)
			: base(token, name)
		{
			DeclaringType = declaringType;
		}

		public MetaUnresolvedType DeclaringType { get; }
		public override void Apply(IMetaTypeVisitor visitor) => visitor.Visit(this);

		public override bool TryGetValue(ICorDebugValue value, ReadOnlyCollection<MetaTypeBase> typeArgs, out object result)
		{
			result = null;
			return false;
		}
	}
}
