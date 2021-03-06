// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace WAYWF.Agent.Data
{
	public sealed class RuntimeNullValue : RuntimeValue
	{
		public static RuntimeValue Instance { get; } = new RuntimeNullValue();

		RuntimeNullValue()
		{
		}

		public override void Apply(IRuntimeValueVisitor visitor) => visitor.Visit(this);
	}
}
