// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using WAYWF.Agent.CorDebugApi;

namespace WAYWF.Agent.MetaCache
{
	abstract class MetaTypeBase
	{
		protected MetaTypeBase()
		{
		}

		public abstract void Apply(IMetaTypeVisitor visitor);

		public virtual bool TryGetValue(ICorDebugValue value, out object result)
		{
			result = null;
			return false;
		}
	}
}
