// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using WAYWF.Agent.CorDebugApi;

namespace WAYWF.Agent.MetaCache
{
	[DebuggerDisplay("Type: {Name,nq}")]
	class MetaResolvedType : MetaType
	{
		public MetaResolvedType(MetaModule module, MetaDataToken token, MetaResolvedType declaringType, string name, int typeArgs)
			: base(token, name)
		{
			Module = module;
			DeclaringType = declaringType;
			TypeArgs = typeArgs;
		}

		public MetaModule Module { get; }
		public MetaResolvedType DeclaringType { get; }
		public int TypeArgs { get; }

		public override void Apply(IMetaTypeVisitor visitor) => visitor.Visit(this);

		public override bool TryGetValue(ICorDebugValue value, ReadOnlyCollection<MetaTypeBase> typeArgs, out object result)
		{
			if (TypeArgs == 0) throw new InvalidOperationException("This overload is only valid for generic types");

			result = null;
			return false;
		}
	}
}
