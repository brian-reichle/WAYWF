// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace WAYWF.Api
{
	public sealed class CaptureConfig
	{
		public bool WalkHeap { get; set; }
		public int WaitSeconds { get; set; }
		public bool Verbose { get; set; }
	}
}
