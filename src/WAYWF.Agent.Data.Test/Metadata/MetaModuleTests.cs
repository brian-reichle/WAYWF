// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test;

[TestFixture]
public class MetaModuleTests
{
	[Test]
	public void Constructor_StoresPropertiesVerbatim()
	{
		var assembly = WellKnownMetaModules.SomeAssembly;
		var moduleId = Identity.NewSource().New();
		var path = @"C:\libs\MyModule.dll";
		var name = "MyModule.dll";
		var mvid = Guid.NewGuid();

		var module = new MetaModule(
			assembly: assembly,
			moduleId: moduleId,
			path: path,
			name: name,
			isInMemory: true,
			isDynamic: false,
			mvid: mvid);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(module.Assembly, Is.SameAs(assembly));
			Assert.That(module.ModuleID, Is.EqualTo(moduleId));
			Assert.That(module.Path, Is.EqualTo(path));
			Assert.That(module.Name, Is.EqualTo(name));
			Assert.That(module.IsInMemory, Is.True);
			Assert.That(module.IsDynamic, Is.False);
			Assert.That(module.MVID, Is.EqualTo(mvid));
		}
	}
}
