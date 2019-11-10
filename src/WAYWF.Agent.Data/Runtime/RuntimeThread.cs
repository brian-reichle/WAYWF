// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;
using System.Diagnostics;

namespace WAYWF.Agent.Data
{
	[DebuggerDisplay("Thread: {ThreadID}")]
	public sealed class RuntimeThread
	{
		public RuntimeThread(int threadId, RuntimeThreadStates userState, ImmutableArray<RuntimeFrameChain> chains, ImmutableArray<RuntimeBlockingObject> blockingObject)
		{
			ThreadID = threadId;
			UserState = userState;
			Chains = chains;
			BlockingObject = blockingObject;
		}

		public int ThreadID { get; }
		public RuntimeThreadStates UserState { get; }
		public ImmutableArray<RuntimeFrameChain> Chains { get; }
		public ImmutableArray<RuntimeBlockingObject> BlockingObject { get; }
	}
}
