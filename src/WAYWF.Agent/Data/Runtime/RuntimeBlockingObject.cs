// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using WAYWF.Agent.CorDebugApi;

namespace WAYWF.Agent.Data
{
	sealed class RuntimeBlockingObject
	{
		public RuntimeBlockingObject(RuntimeValue value, int ownerId, int timeout, CorDebugBlockingReason blockingReason)
		{
			Value = value;
			OwnerId = ownerId;
			Timeout = timeout;
			BlockingReason = blockingReason;
		}

		public RuntimeValue Value { get; }
		public int OwnerId { get; }
		public int Timeout { get; }
		public CorDebugBlockingReason BlockingReason { get; }
	}
}
