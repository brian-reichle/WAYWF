// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace WAYWF.Agent.CorDebugApi
{
	[Flags]
	enum CorDebugUserState
	{
		/// <summary>
		/// A termination of the thread has been requested.
		/// </summary>
		USER_STOP_REQUESTED = 0x01,

		/// <summary>
		/// A suspension of the thread has been requested.
		/// </summary>
		USER_SUSPEND_REQUESTED = 0x02,

		/// <summary>
		/// The thread is running in the background.
		/// </summary>
		USER_BACKGROUND = 0x04,

		/// <summary>
		/// The thread has not started executing.
		/// </summary>
		USER_UNSTARTED = 0x08,

		/// <summary>
		/// The thread has been terminated.
		/// </summary>
		USER_STOPPED = 0x10,

		/// <summary>
		/// The thread is waiting for another thread to complete a task.
		/// </summary>
		USER_WAIT_SLEEP_JOIN = 0x20,

		/// <summary>
		/// The thread has been suspended.
		/// </summary>
		USER_SUSPENDED = 0x40,

		/// <summary>
		/// The thread is at an unsafe point.
		/// </summary>
		/// <remarks>
		/// That is, the thread is at a point in execution where it may block garbage collection.
		/// Debug events may be dispatched from unsafe points, but suspending a thread at an unsafe point will very likely
		/// cause a deadlock until the thread is resumed. The safe and unsafe points are determined by the just-in-time
		/// (JIT) and garbage collection implementation.
		/// </remarks>
		USER_UNSAFE_POINT = 0x80,

		/// <summary>
		/// The thread is from the thread pool.
		/// </summary>
		USER_THREADPOOL = 0x100,
	}
}
