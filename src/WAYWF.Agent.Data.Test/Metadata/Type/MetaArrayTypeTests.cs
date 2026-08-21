// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Linq;
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test;

[TestFixture]
public class MetaArrayTypeTests
{
	[Test]
	public void Constructor_SetsElementTypeAndRank()
	{
		var elementType = MetaKnownType.Int32;
		var arrayType = new MetaArrayType(elementType, 2);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(arrayType.ElementType, Is.SameAs(elementType));
			Assert.That(arrayType.Rank, Is.EqualTo(2));
		}
	}

	[Test]
	public void Apply_VisitorDispatchesCorrectly()
	{
		var arrayType = new MetaArrayType(MetaKnownType.Int32, 1);
		var visitor = new DummyLogMetaTypeVisitor();

		arrayType.Apply(visitor);

		using (Assert.EnterMultipleScope())
		{
			var record = visitor.Records.Single();
			Assert.That(record.Method.Name, Is.EqualTo(nameof(IMetaTypeVisitor.VisitArray)));
			Assert.That(record.Type, Is.SameAs(arrayType));
		}
	}

	[Test]
	public void ApplyWithArg_VisitorDispatchesAndReturnsCorrectly()
	{
		var arrayType = new MetaArrayType(MetaKnownType.Int32, 1);
		var visitor = new DummyLogMetaTypeVisitor<string, int>(42);

		var result = arrayType.Apply(visitor, "test");
		Assert.That(result, Is.EqualTo(42), "Visitor return value should be propagated.");

		using (Assert.EnterMultipleScope())
		{
			var record = visitor.Records.Single();
			Assert.That(record.Method.Name, Is.EqualTo(nameof(IMetaTypeVisitor.VisitArray)));
			Assert.That(record.Type, Is.SameAs(arrayType));
			Assert.That(record.Argument, Is.EqualTo("test"));
		}
	}
}
