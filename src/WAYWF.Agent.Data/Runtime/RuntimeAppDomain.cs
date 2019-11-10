// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;
using System.Diagnostics;

namespace WAYWF.Agent.Data
{
	[DebuggerDisplay("AppDomain-{AppDomainID}: {Name,nq}")]
	public sealed class RuntimeAppDomain
	{
		public RuntimeAppDomain(int appDomainId, string name, ImmutableArray<MetaModule> modules)
		{
			AppDomainID = appDomainId;
			Name = name;
			Modules = modules;
		}

		public int AppDomainID { get; }
		public string Name { get; }
		public ImmutableArray<MetaModule> Modules { get; }
	}
}
