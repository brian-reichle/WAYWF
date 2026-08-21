// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Linq;
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test;

[TestFixture]
public class MetaNullableTypeTests
{
	[Test]
	public void Constructor_SetsProperties()
	{
		var module = WellKnownMetaModules.SomeModule;
		var token = new MetaDataToken(0x02000001);
		var hasValueToken = new MetaDataToken(0x04000001);
		var valueToken = new MetaDataToken(0x04000002);
		var nullableType = new MetaNullableType(module, token, hasValueToken, valueToken);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(nullableType.Module, Is.SameAs(module));
			Assert.That(nullableType.Token, Is.EqualTo(token));
			Assert.That(nullableType.HasValueToken, Is.EqualTo(hasValueToken));
			Assert.That(nullableType.ValueToken, Is.EqualTo(valueToken));
			Assert.That(nullableType.DeclaringType, Is.Null);
			Assert.That(nullableType.Name, Is.EqualTo(MetaNullableType.TypeName));
			Assert.That(nullableType.TypeArgs, Is.EqualTo(1));
		}
	}

	[Test]
	public void Apply_VisitorDispatchesCorrectly()
	{
		var nullableType = new MetaNullableType(WellKnownMetaModules.SomeModule, new MetaDataToken(0x02000001), new MetaDataToken(0x04000001), new MetaDataToken(0x04000002));
		var visitor = new DummyLogMetaTypeVisitor();

		nullableType.Apply(visitor);

		using (Assert.EnterMultipleScope())
		{
			var record = visitor.Records.Single();
			Assert.That(record.Method.Name, Is.EqualTo(nameof(IMetaTypeVisitor.VisitNullable)));
			Assert.That(record.Type, Is.SameAs(nullableType));
		}
	}

	[Test]
	public void ApplyWithArg_VisitorDispatchesAndReturnsCorrectly()
	{
		var nullableType = new MetaNullableType(WellKnownMetaModules.SomeModule, new MetaDataToken(0x02000001), new MetaDataToken(0x04000001), new MetaDataToken(0x04000002));
		var visitor = new DummyLogMetaTypeVisitor<string, int>(55);

		var result = nullableType.Apply(visitor, "nullable");
		Assert.That(result, Is.EqualTo(55), "Visitor return value should be propagated.");

		using (Assert.EnterMultipleScope())
		{
			var record = visitor.Records.Single();
			Assert.That(record.Method.Name, Is.EqualTo(nameof(IMetaTypeVisitor.VisitNullable)));
			Assert.That(record.Type, Is.SameAs(nullableType));
			Assert.That(record.Argument, Is.EqualTo("nullable"));
		}
	}
}
