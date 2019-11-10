// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;

namespace WAYWF.Agent.Data
{
	public sealed class RuntimeRcwValue : RuntimeIdentifiedValue
	{
		public RuntimeRcwValue(Identity id, MetaTypeBase type, ImmutableArray<MetaTypeBase> interfaceTypes, ImmutableArray<RuntimeNativeInterface> interfacePointers)
			: base(id, type)
		{
			InterfaceTypes = interfaceTypes;
			InterfacePointers = interfacePointers;
		}

		public ImmutableArray<MetaTypeBase> InterfaceTypes { get; }
		public ImmutableArray<RuntimeNativeInterface> InterfacePointers { get; }

		public override void Apply(IRuntimeValueVisitor visitor) => visitor.Visit(this);
	}
}
