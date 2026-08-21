// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Linq;
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test;

[TestFixture]
public class MetaGCHandleTypeTests
{
	[Test]
	public void Constructor_SetsProperties()
	{
		var module = WellKnownMetaModules.SomeModule;
		var token = new MetaDataToken(0x02000001);
		var handleField = new MetaDataToken(0x04000001);
		var gcHandleType = new MetaGCHandleType(module, token, handleField);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(gcHandleType.Module, Is.SameAs(module));
			Assert.That(gcHandleType.Token, Is.EqualTo(token));
			Assert.That(gcHandleType.HandleField, Is.EqualTo(handleField));
			Assert.That(gcHandleType.DeclaringType, Is.Null);
			Assert.That(gcHandleType.Name, Is.EqualTo(MetaGCHandleType.TypeName));
			Assert.That(gcHandleType.TypeArgs, Is.Zero);
		}
	}

	[Test]
	public void Apply_VisitorDispatchesCorrectly()
	{
		var gcHandleType = new MetaGCHandleType(WellKnownMetaModules.SomeModule, new MetaDataToken(0x02000001), new MetaDataToken(0x04000001));
		var visitor = new DummyLogMetaTypeVisitor();

		gcHandleType.Apply(visitor);

		using (Assert.EnterMultipleScope())
		{
			var record = visitor.Records.Single();
			Assert.That(record.Method.Name, Is.EqualTo(nameof(IMetaTypeVisitor.VisitGCHandle)));
			Assert.That(record.Type, Is.SameAs(gcHandleType));
		}
	}

	[Test]
	public void ApplyWithArg_VisitorDispatchesAndReturnsCorrectly()
	{
		var gcHandleType = new MetaGCHandleType(WellKnownMetaModules.SomeModule, new MetaDataToken(0x02000001), new MetaDataToken(0x04000001));
		var visitor = new DummyLogMetaTypeVisitor<string, int>(99);

		var result = gcHandleType.Apply(visitor, "gchandle");
		Assert.That(result, Is.EqualTo(99), "Visitor return value should be propagated.");

		using (Assert.EnterMultipleScope())
		{
			var record = visitor.Records.Single();
			Assert.That(record.Method.Name, Is.EqualTo(nameof(IMetaTypeVisitor.VisitGCHandle)));
			Assert.That(record.Type, Is.SameAs(gcHandleType));
			Assert.That(record.Argument, Is.EqualTo("gchandle"));
		}
	}
}
