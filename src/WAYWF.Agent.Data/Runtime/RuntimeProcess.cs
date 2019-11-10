// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Immutable;
using System.Diagnostics;

namespace WAYWF.Agent.Data
{
	[DebuggerDisplay("Process: {Native.ProcessID}")]
	public sealed class RuntimeProcess
	{
		public RuntimeProcess(
			CaptureOptions options,
			RuntimeNative native,
			Version clrVersion,
			ImmutableArray<RuntimeAppDomain> appDomains,
			ImmutableArray<RuntimeThread> threads,
			ImmutableArray<SourceDocument> documents,
			ImmutableArray<RuntimeValue> referenceValues,
			ImmutableArray<PendingStateMachineTask> pendingTasks)
		{
			Options = options;
			Native = native;
			ClrVersion = clrVersion;
			DateTime = DateTimeOffset.Now;
			AppDomains = appDomains;
			Threads = threads;
			Documents = documents;
			ReferenceValues = referenceValues;
			PendingTasks = pendingTasks;
		}

		public CaptureOptions Options { get; }
		public RuntimeNative Native { get; }
		public Version ClrVersion { get; }
		public DateTimeOffset DateTime { get; }
		public ImmutableArray<RuntimeAppDomain> AppDomains { get; }
		public ImmutableArray<RuntimeThread> Threads { get; }
		public ImmutableArray<SourceDocument> Documents { get; }
		public ImmutableArray<RuntimeValue> ReferenceValues { get; }
		public ImmutableArray<PendingStateMachineTask> PendingTasks { get; }
	}
}
