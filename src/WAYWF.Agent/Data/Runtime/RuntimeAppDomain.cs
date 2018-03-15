// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.ObjectModel;
using System.Diagnostics;
using WAYWF.Agent.MetaCache;

namespace WAYWF.Agent.Data
{
	[DebuggerDisplay("AppDomain-{AppDomainID}: {Name,nq}")]
	sealed class RuntimeAppDomain
	{
		public RuntimeAppDomain(int appDomainId, string name, MetaAssembly[] assembly)
		{
			AppDomainID = appDomainId;
			Name = name;
			Assembly = assembly.MakeReadOnly();
		}

		public int AppDomainID { get; }
		public string Name { get; }
		public ReadOnlyCollection<MetaAssembly> Assembly { get; }
	}
}
