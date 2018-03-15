// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.ObjectModel;
using System.Diagnostics;
using WAYWF.Agent.CorDebugApi;

namespace WAYWF.Agent.MetaCache
{
	[DebuggerDisplay("Type: {Name,nq}")]
	abstract class MetaType : MetaTypeBase
	{
		public MetaType(MetaDataToken token, string name)
		{
			Token = token;
			Name = name;
		}

		public MetaDataToken Token { get; }
		public string Name { get; }

		public abstract bool TryGetValue(ICorDebugValue value, ReadOnlyCollection<MetaTypeBase> typeArgs, out object result);
	}
}
