// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace WAYWF.Agent.CorDebugApi
{
	enum CorDebugBlockingReason
	{
		BLOCKING_NONE = 0x0,

		/// <summary>
		/// A thread is trying to acquire the critical section that is associated with the monitor lock on an object.
		/// Typically, this occurs when you call one of the Monitor.Enter or Monitor.TryEnter methods.
		/// </summary>
		BLOCKING_MONITOR_CRITICAL_SECTION = 0x1,

		/// <summary>
		/// A thread is waiting on the event that is associated with a monitor lock for an object.
		/// Typically, this occurs when you call one of the System.Threading.Monitor Wait methods.
		/// </summary>
		BLOCKING_MONITOR_EVENT = 0x2
	}
}
