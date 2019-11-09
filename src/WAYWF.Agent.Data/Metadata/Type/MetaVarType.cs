// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;

namespace WAYWF.Agent.Data
{
	[DebuggerDisplay("Generic Var: Index={Index}, Method={Method}")]
	public sealed class MetaVarType : MetaTypeBase
	{
		public MetaVarType(bool method, int index)
		{
			Method = method;
			Index = index;
		}

		public bool Method { get; }
		public int Index { get; }

		public override void Apply(IMetaTypeVisitor visitor) => visitor.VisitVar(this);
		public override TResult Apply<TArg, TResult>(IMetaTypeVisitor<TArg, TResult> visitor, TArg arg) => visitor.VisitVar(this, arg);
	}
}
