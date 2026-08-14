// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test
{
	[TestFixture]
	public class RuntimeUserTests
	{
		[Test]
		public void Constructor_SetsProperties()
		{
			var user = "johndoe";
			var domain = "WORKGROUP";

			var runtimeUser = new RuntimeUser(user, domain);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(runtimeUser.User, Is.EqualTo(user));
				Assert.That(runtimeUser.Domain, Is.EqualTo(domain));
			}
		}
	}
}
