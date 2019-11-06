// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace WAYWF.Agent.Data
{
	public sealed class RuntimeBlockingObject
	{
		public RuntimeBlockingObject(RuntimeValue value, int ownerId, int timeout, RuntimeBlockingReason blockingReason)
		{
			Value = value;
			OwnerId = ownerId;
			Timeout = timeout;
			BlockingReason = blockingReason;
		}

		public RuntimeValue Value { get; }
		public int OwnerId { get; }
		public int Timeout { get; }
		public RuntimeBlockingReason BlockingReason { get; }
	}
}
