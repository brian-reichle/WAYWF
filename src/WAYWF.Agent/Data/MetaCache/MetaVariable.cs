// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;

namespace WAYWF.Agent.MetaCache
{
	[DebuggerDisplay("Variable: {Name,nq}")]
	sealed class MetaVariable
	{
		public MetaVariable(MetaTypeBase type, string name, bool isByRef, bool pinned)
		{
			Type = type;
			Name = name;
			IsByRef = isByRef;
			Pinned = pinned;
		}

		public MetaTypeBase Type { get; }
		public string Name { get; }
		public bool IsByRef { get; }
		public bool Pinned { get; }
	}
}
