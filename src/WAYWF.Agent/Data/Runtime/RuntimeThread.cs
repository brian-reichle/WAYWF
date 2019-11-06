// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace WAYWF.Agent.Data
{
	[DebuggerDisplay("Thread: {ThreadID}")]
	sealed class RuntimeThread
	{
		public RuntimeThread(int threadId, RuntimeThreadStates userState, RuntimeFrameChain[] chains, RuntimeBlockingObject[] blockingObject)
		{
			ThreadID = threadId;
			UserState = userState;
			Chains = chains.MakeReadOnly();
			BlockingObject = blockingObject.MakeReadOnly();
		}

		public int ThreadID { get; }
		public RuntimeThreadStates UserState { get; }
		public ReadOnlyCollection<RuntimeFrameChain> Chains { get; }
		public ReadOnlyCollection<RuntimeBlockingObject> BlockingObject { get; }
	}
}
