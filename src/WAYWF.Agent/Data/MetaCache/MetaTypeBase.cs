// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace WAYWF.Agent.MetaCache
{
	abstract class MetaTypeBase
	{
		protected MetaTypeBase()
		{
		}

		public abstract void Apply(IMetaTypeVisitor visitor);
		public abstract TResult Apply<TArg, TResult>(IMetaTypeVisitor<TArg, TResult> visitor, TArg arg);
	}
}
