// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test
{
	[TestFixture]
	public class RuntimeNativeTests
	{
		[Test]
		public void Constructor_SetsProperties()
		{
			var processId = 1234;
			var imageName = "app.exe";
			var user = new RuntimeUser("user", "domain");
			var windows = ImmutableArray<RuntimeWindow>.Empty;

			var native = new RuntimeNative(processId, imageName, user, windows);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(native.ProcessID, Is.EqualTo(processId));
				Assert.That(native.ImageName, Is.EqualTo(imageName));
				Assert.That(native.User, Is.EqualTo(user));
				Assert.That(native.Windows, Is.EqualTo(windows));
			}
		}
	}
}
