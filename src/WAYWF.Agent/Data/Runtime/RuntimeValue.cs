// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace WAYWF.Agent.Data
{
	abstract class RuntimeValue
	{
		protected RuntimeValue()
		{
			ReferenceCount = 1;
		}

		public int ReferenceCount { get; set; }

		public abstract void Apply(IRuntimeValueVisitor visitor);
	}
}
