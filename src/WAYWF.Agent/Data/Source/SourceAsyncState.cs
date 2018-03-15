// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace WAYWF.Agent.Source
{
	sealed class SourceAsyncState
	{
		public SourceAsyncState(int yieldOffset, SourceRef yieldSource)
		{
			YieldOffset = yieldOffset;
			YieldSource = yieldSource;
		}

		public int YieldOffset { get; }
		public SourceRef YieldSource { get; }
	}
}
