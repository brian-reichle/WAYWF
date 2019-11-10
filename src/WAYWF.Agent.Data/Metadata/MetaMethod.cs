// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;
using System.Diagnostics;

namespace WAYWF.Agent.Data
{
	[DebuggerDisplay("Method: {Name,nq}")]
	public sealed class MetaMethod
	{
		public MetaMethod(MetaDataToken token, MetaModule module, MetaResolvedType declaringType, string name, MetaMethodSignature signature, ImmutableArray<MetaVariable> locals)
		{
			Token = token;
			Module = module;
			DeclaringType = declaringType;
			Name = name;
			Signature = signature;
			Locals = locals;
		}

		public MetaDataToken Token { get; }
		public MetaModule Module { get; }
		public MetaResolvedType DeclaringType { get; }
		public string Name { get; }
		public MetaMethodSignature Signature { get; }
		public ImmutableArray<MetaVariable> Locals { get; }
	}
}
