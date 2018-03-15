// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Diagnostics;

namespace WAYWF.Agent.MetaCache
{
	[DebuggerDisplay("Module-{ModuleID}: {Name,nq}")]
	sealed class MetaModule
	{
		public MetaModule(MetaAssembly assembly, Identity moduleId, string path, string name, bool isInMemory, bool isDynamic, Guid mvid)
		{
			Assembly = assembly;
			ModuleID = moduleId;
			Path = path;
			Name = name;
			IsInMemory = isInMemory;
			IsDynamic = isDynamic;
			MVID = mvid;
		}

		public MetaAssembly Assembly { get; }
		public Identity ModuleID { get; }
		public string Path { get; }
		public string Name { get; }
		public bool IsInMemory { get; }
		public bool IsDynamic { get; }
		public Guid MVID { get; }
	}
}
