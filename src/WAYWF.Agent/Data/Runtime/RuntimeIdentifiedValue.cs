// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using WAYWF.Agent.MetaCache;

namespace WAYWF.Agent.Data
{
	abstract class RuntimeIdentifiedValue : RuntimeValue
	{
		protected RuntimeIdentifiedValue(Identity id, MetaTypeBase type)
		{
			ID = id;
			Type = type;
		}

		public Identity ID { get; }
		public MetaTypeBase Type { get; }

		public sealed override string ToString() => ID.ToString();
	}
}
