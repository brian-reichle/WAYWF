// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using WAYWF.Agent.PendingTasks;
using WAYWF.Agent.Source;

namespace WAYWF.Agent.Data
{
	[DebuggerDisplay("Process: {Native.ProcessID}")]
	sealed class RuntimeProcess
	{
		public RuntimeProcess(
			RuntimeNative native,
			Version clrVersion,
			RuntimeAppDomain[] appDomains,
			RuntimeThread[] threads,
			SourceDocument[] documents,
			RuntimeValue[] referenceValues,
			PendingStateMachineTask[] pendingTasks)
		{
			Native = native;
			ClrVersion = clrVersion;
			DateTime = DateTimeOffset.Now;
			AppDomains = appDomains.MakeReadOnly();
			Threads = threads.MakeReadOnly();
			Documents = documents.MakeReadOnly();
			ReferenceValues = referenceValues.MakeReadOnly();
			PendingTasks = pendingTasks.MakeReadOnly();
		}

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
