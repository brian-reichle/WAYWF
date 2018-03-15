// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;

namespace WAYWF.Agent.MetaCache
{
	[DebuggerDisplay("Generic Var: Index={Index}, Method={Method}")]
	sealed class MetaVarType : MetaTypeBase
	{
		public MetaVarType(bool method, int index)
		{
			Method = method;
			Index = index;
		}

		public bool Method { get; }
		public int Index { get; }
		public override void Apply(IMetaTypeVisitor visitor) => visitor.Visit(this);
	}
}
