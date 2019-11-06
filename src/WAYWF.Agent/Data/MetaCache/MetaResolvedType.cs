// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;

namespace WAYWF.Agent.MetaCache
{
	[DebuggerDisplay("Type: {Name,nq}")]
	abstract class MetaResolvedType : MetaType
	{
		protected MetaResolvedType(MetaModule module, MetaDataToken token, MetaResolvedType declaringType, string name, int typeArgs)
			: base(token, name)
		{
			Module = module;
			DeclaringType = declaringType;
			TypeArgs = typeArgs;
		}

		public MetaModule Module { get; }
		public MetaResolvedType DeclaringType { get; }
		public int TypeArgs { get; }
	}
}
