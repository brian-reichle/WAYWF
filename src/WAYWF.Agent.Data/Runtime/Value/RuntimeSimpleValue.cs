// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace WAYWF.Agent.Data
{
	public sealed class RuntimeSimpleValue : RuntimeIdentifiedValue
	{
		public RuntimeSimpleValue(Identity id, MetaTypeBase type, object value)
			: base(id, type)
		{
			Value = value;
		}

		public object Value { get; }

		public override void Apply(IRuntimeValueVisitor visitor) => visitor.Visit(this);
	}
}
