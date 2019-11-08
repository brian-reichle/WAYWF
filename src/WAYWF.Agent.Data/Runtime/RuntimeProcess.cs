// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.ObjectModel;
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
			RuntimeAppDomain[] appDomains,
			RuntimeThread[] threads,
			SourceDocument[] documents,
			RuntimeValue[] referenceValues,
			PendingStateMachineTask[] pendingTasks)
		{
			Options = options;
			Native = native;
			ClrVersion = clrVersion;
			DateTime = DateTimeOffset.Now;
			AppDomains = appDomains.MakeReadOnly();
			Threads = threads.MakeReadOnly();
			Documents = documents.MakeReadOnly();
			ReferenceValues = referenceValues.MakeReadOnly();
			PendingTasks = pendingTasks.MakeReadOnly();
		}

		public CaptureOptions Options { get; }
		public RuntimeNative Native { get; }
		public Version ClrVersion { get; }
		public DateTimeOffset DateTime { get; }
		public ReadOnlyCollection<RuntimeAppDomain> AppDomains { get; }
		public ReadOnlyCollection<RuntimeThread> Threads { get; }
		public ReadOnlyCollection<SourceDocument> Documents { get; }
		public ReadOnlyCollection<RuntimeValue> ReferenceValues { get; }
		public ReadOnlyCollection<PendingStateMachineTask> PendingTasks { get; }
	}
}
