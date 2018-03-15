// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using WAYWF.Agent.CorDebugApi;
using WAYWF.Agent.MetaCache;

namespace WAYWF.Agent.Data
{
	sealed class RuntimePointerValue : RuntimeValue
	{
		public RuntimePointerValue(MetaTypeBase type, CORDB_ADDRESS address, RuntimeValue value)
		{
			Type = type;
			Address = address;
			Value = value;
		}

		public MetaTypeBase Type { get; }
		public CORDB_ADDRESS Address { get; }
		public RuntimeValue Value { get; }

		public override void Apply(IRuntimeValueVisitor visitor) => visitor.Visit(this);
	}
}
