// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test;

[TestFixture]
public class MetaAssemblyTests
{
	[Test]
	public void Constructor_StoresPropertiesVerbatim()
	{
		var version = new Version(1, 2, 3, 4);
		var assembly = new MetaAssembly(
			path: @"C:\libs\MyLib.dll",
			name: "MyLib",
			version: version,
			publicKeyToken: 0x123456789ABCDEF0L,
			locale: "en-US");

		using (Assert.EnterMultipleScope())
		{
			Assert.That(assembly.Path, Is.EqualTo(@"C:\libs\MyLib.dll"));
			Assert.That(assembly.Name, Is.EqualTo("MyLib"));
			Assert.That(assembly.Version, Is.EqualTo(version));
			Assert.That(assembly.PublicKeyToken, Is.EqualTo(0x123456789ABCDEF0L));
			Assert.That(assembly.Locale, Is.EqualTo("en-US"));
			Assert.That(assembly.IsCorLib, Is.False);
		}
	}

	[TestCase("mscorlib", true)]
	[TestCase("MSCORLIB", false)]
	[TestCase("System.Private.CoreLib", false)]
	[TestCase("MyLib", false)]
	[TestCase("", false)]
	public void IsCorLib_EvaluatesName(string name, bool expectedIsCorLib)
	{
		var assembly = new MetaAssembly(
			path: "path",
			name: name,
			version: new Version(1, 0),
			publicKeyToken: null,
			locale: null);

		Assert.That(assembly.IsCorLib, Is.EqualTo(expectedIsCorLib));
	}
}
