// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.ObjectModel;
using WAYWF.Agent.MetaCache;

namespace WAYWF.Agent.Data
{
	sealed class RuntimeRcwValue : RuntimeIdentifiedValue
	{
		public RuntimeRcwValue(Identity id, MetaTypeBase type, MetaTypeBase[] interfaceTypes, RuntimeNativeInterface[] interfacePointers)
			: base(id, type)
		{
			InterfaceTypes = interfaceTypes.MakeReadOnly();
			InterfacePointers = interfacePointers.MakeReadOnly();
		}

		public ReadOnlyCollection<MetaTypeBase> InterfaceTypes { get; }
		public ReadOnlyCollection<RuntimeNativeInterface> InterfacePointers { get; }
		public override void Apply(IRuntimeValueVisitor visitor) => visitor.Visit(this);
	}
}
