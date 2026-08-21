// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Linq;
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test;

[TestFixture]
public class MetaUnresolvedTypeTests
{
	[Test]
	public void Constructor_SetsProperties()
	{
		var token = new MetaDataToken(0x02000001);
		var declaringType = new MetaUnresolvedType(new MetaDataToken(0x02000002), null, "DeclaringType");
		var type = new MetaUnresolvedType(token, declaringType, "TestType");

		using (Assert.EnterMultipleScope())
		{
			Assert.That(type.Token, Is.EqualTo(token));
			Assert.That(type.DeclaringType, Is.SameAs(declaringType));
			Assert.That(type.Name, Is.EqualTo("TestType"));
		}
	}

	[Test]
	public void Apply_VisitorDispatchesCorrectly()
	{
		var type = new MetaUnresolvedType(new MetaDataToken(0x02000001), null, "TestType");
		var visitor = new DummyLogMetaTypeVisitor();

		type.Apply(visitor);

		using (Assert.EnterMultipleScope())
		{
			var record = visitor.Records.Single();
			Assert.That(record.Method.Name, Is.EqualTo(nameof(IMetaTypeVisitor.VisitUnresolved)));
			Assert.That(record.Type, Is.SameAs(type));
		}
	}

	[Test]
	public void ApplyWithArg_VisitorDispatchesAndReturnsCorrectly()
	{
		var type = new MetaUnresolvedType(new MetaDataToken(0x02000001), null, "TestType");
		var visitor = new DummyLogMetaTypeVisitor<string, int>(109);

		var result = type.Apply(visitor, "unresolved");
		Assert.That(result, Is.EqualTo(109), "Visitor return value should be propagated.");

		using (Assert.EnterMultipleScope())
		{
			var record = visitor.Records.Single();
			Assert.That(record.Method.Name, Is.EqualTo(nameof(IMetaTypeVisitor.VisitUnresolved)));
			Assert.That(record.Type, Is.SameAs(type));
			Assert.That(record.Argument, Is.EqualTo("unresolved"));
		}
	}
}
