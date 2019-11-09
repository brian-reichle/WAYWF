// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace WAYWF.Agent.Data
{
	[Flags]
	public enum RuntimeThreadStates
	{
		None = 0,
		NotStarted = 1 << 0,
		Background = 1 << 1,
		ThreadPool = 1 << 2,

		Stopping = 1 << 3,
		Stopped = 1 << 4,

		Suspending = 1 << 5,
		Suspended = 1 << 6,

		WaitSleepJoin = 1 << 7,
	}
}
