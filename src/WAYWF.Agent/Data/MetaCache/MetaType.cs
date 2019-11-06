// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;

namespace WAYWF.Agent.MetaCache
{
	[DebuggerDisplay("Type: {Name,nq}")]
	abstract class MetaType : MetaTypeBase
	{
		protected MetaType(MetaDataToken token, string name)
		{
			Token = token;
			Name = name;
		}

		public MetaDataToken Token { get; }
		public string Name { get; }
	}
}
