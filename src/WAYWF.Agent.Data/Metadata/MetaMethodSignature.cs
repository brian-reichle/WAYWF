// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;
using System.Reflection;

namespace WAYWF.Agent.Data
{
	public sealed class MetaMethodSignature
	{
		public MetaMethodSignature(CallingConventions callingConventions, int typeArgs, MetaVariable resultParam, ImmutableArray<MetaVariable> parameters)
		{
			CallingConventions = callingConventions;
			TypeArg = typeArgs;
			ResultParam = resultParam;
			Parameters = parameters;
		}

		public CallingConventions CallingConventions { get; }
		public int TypeArg { get; }
		public MetaVariable ResultParam { get; }
		public ImmutableArray<MetaVariable> Parameters { get; }
	}
}
