// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.ObjectModel;
using System.Reflection;

namespace WAYWF.Agent.MetaCache
{
	sealed class MetaMethodSignature
	{
		public MetaMethodSignature(CallingConventions callingConventions, int typeArgs, MetaVariable resultParam, MetaVariable[] parameters)
		{
			CallingConventions = callingConventions;
			TypeArg = typeArgs;
			ResultParam = resultParam;
			Parameters = parameters.MakeReadOnly();
		}

		public CallingConventions CallingConventions { get; }
		public int TypeArg { get; }
		public MetaVariable ResultParam { get; }
		public ReadOnlyCollection<MetaVariable> Parameters { get; }
	}
}
