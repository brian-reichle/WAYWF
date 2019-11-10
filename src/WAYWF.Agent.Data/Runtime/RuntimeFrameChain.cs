// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;

namespace WAYWF.Agent.Data
{
	public sealed class RuntimeFrameChain
	{
		public RuntimeFrameChain(RuntimeFrameChainReason reason, ImmutableArray<RuntimeFrame> frames)
		{
			Reason = reason;
			Frames = frames;
		}

		public RuntimeFrameChainReason Reason { get; }
		public ImmutableArray<RuntimeFrame> Frames { get; }
	}
}
