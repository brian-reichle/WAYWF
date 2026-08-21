// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Linq;
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test;

[TestFixture]
public class MetaPointerTypeTests
{
	[Test]
	public void Constructor_SetsElementType()
	{
		var elementType = MetaKnownType.Int32;
		var pointerType = new MetaPointerType(elementType);

		Assert.That(pointerType.ElementType, Is.SameAs(elementType));
	}

	[Test]
	public void Apply_VisitorDispatchesCorrectly()
	{
		var pointerType = new MetaPointerType(MetaKnownType.Int32);
		var visitor = new DummyLogMetaTypeVisitor();

		pointerType.Apply(visitor);

		using (Assert.EnterMultipleScope())
		{
			var record = visitor.Records.Single();
			Assert.That(record.Method.Name, Is.EqualTo(nameof(IMetaTypeVisitor.VisitPointer)));
			Assert.That(record.Type, Is.SameAs(pointerType));
		}
	}

	[Test]
	public void ApplyWithArg_VisitorDispatchesAndReturnsCorrectly()
	{
		var pointerType = new MetaPointerType(MetaKnownType.Int32);
		var visitor = new DummyLogMetaTypeVisitor<string, int>(77);

		var result = pointerType.Apply(visitor, "pointer");
		Assert.That(result, Is.EqualTo(77), "Visitor return value should be propagated.");

		using (Assert.EnterMultipleScope())
		{
			var record = visitor.Records.Single();
			Assert.That(record.Method.Name, Is.EqualTo(nameof(IMetaTypeVisitor.VisitPointer)));
			Assert.That(record.Type, Is.SameAs(pointerType));
			Assert.That(record.Argument, Is.EqualTo("pointer"));
		}
	}
}
