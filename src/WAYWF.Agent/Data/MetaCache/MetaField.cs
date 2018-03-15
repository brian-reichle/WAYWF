// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace WAYWF.Agent.MetaCache
{
	sealed class MetaField
	{
		public MetaField(MetaDataToken token, MetaTypeBase type, string name)
		{
			Token = token;
			Type = type;
			Name = name;
		}

		public MetaDataToken Token { get; }
		public string Name { get; }
		public MetaTypeBase Type { get; }
	}
}
