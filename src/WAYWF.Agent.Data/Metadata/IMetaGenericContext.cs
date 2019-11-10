// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;

namespace WAYWF.Agent.Data
{
	public interface IMetaGenericContext
	{
		int StartOfMethodArgs { get; }
		ImmutableArray<MetaTypeBase> TypeArgs { get; }
	}
}
