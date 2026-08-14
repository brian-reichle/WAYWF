// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Immutable;
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test
{
	[TestFixture]
	public class RuntimeProcessTests
	{
		[Test]
		public void Constructor_SetsProperties()
		{
			var options = new CaptureOptions(true, 10);
			var native = new RuntimeNative(1234, "test.exe", new RuntimeUser("user", "domain"), []);
			var clrVersion = new Version(4, 0, 30319);
			var appDomains = ImmutableArray<RuntimeAppDomain>.Empty;
			var threads = ImmutableArray<RuntimeThread>.Empty;
			var documents = ImmutableArray<SourceDocument>.Empty;
			var referenceValues = ImmutableArray<RuntimeValue>.Empty;
			var pendingTasks = ImmutableArray<PendingStateMachineTask>.Empty;

			var before = DateTimeOffset.Now;
			var process = new RuntimeProcess(options, native, clrVersion, appDomains, threads, documents, referenceValues, pendingTasks);
			var after = DateTimeOffset.Now;

			using (Assert.EnterMultipleScope())
			{
				Assert.That(process.Options, Is.EqualTo(options));
				Assert.That(process.Native, Is.EqualTo(native));
				Assert.That(process.ClrVersion, Is.EqualTo(clrVersion));
				Assert.That(process.AppDomains, Is.EqualTo(appDomains));
				Assert.That(process.Threads, Is.EqualTo(threads));
				Assert.That(process.Documents, Is.EqualTo(documents));
				Assert.That(process.ReferenceValues, Is.EqualTo(referenceValues));
				Assert.That(process.PendingTasks, Is.EqualTo(pendingTasks));
				Assert.That(process.DateTime, Is.InRange(before, after));
			}
		}
	}
}
