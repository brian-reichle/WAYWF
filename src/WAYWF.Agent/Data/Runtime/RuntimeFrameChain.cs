// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.ObjectModel;

namespace WAYWF.Agent.Data
{
	sealed class RuntimeFrameChain
	{
		public RuntimeFrameChain(RuntimeFrameChainReason reason, RuntimeFrame[] frames)
		{
			Reason = reason;
			Frames = frames.MakeReadOnly();
		}

		public RuntimeFrameChainReason Reason { get; }
		public ReadOnlyCollection<RuntimeFrame> Frames { get; }
	}
}
