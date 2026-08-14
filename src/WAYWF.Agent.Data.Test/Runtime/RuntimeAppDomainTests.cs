// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test
{
	[TestFixture]
	public class RuntimeAppDomainTests
	{
		[Test]
		public void Constructor_SetsProperties()
		{
			var appDomainId = 1;
			var name = "DefaultDomain";
			var modules = ImmutableArray<MetaModule>.Empty;

			var appDomain = new RuntimeAppDomain(appDomainId, name, modules);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(appDomain.AppDomainID, Is.EqualTo(appDomainId));
				Assert.That(appDomain.Name, Is.EqualTo(name));
				Assert.That(appDomain.Modules, Is.EqualTo(modules));
			}
		}
	}
}
