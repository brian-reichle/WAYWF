// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace WAYWF.Agent.Data
{
	public sealed class CaptureOptions
	{
		public CaptureOptions(bool walkHeap, int waitSeconds)
		{
			WalkHeap = walkHeap;
			WaitSeconds = waitSeconds;
		}

		public bool WalkHeap { get; }
		public int WaitSeconds { get; }
	}
}
