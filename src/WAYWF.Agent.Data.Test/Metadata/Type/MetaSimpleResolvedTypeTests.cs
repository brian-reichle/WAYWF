// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Linq;
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test
{
	[TestFixture]
	public class MetaSimpleResolvedTypeTests
	{
		[Test]
		public void Constructor_SetsProperties()
		{
			var module = WellKnownMetaModules.SomeModule;
			var token = new MetaDataToken(0x02000001);
			var declaringType = new MetaSimpleResolvedType(module, new MetaDataToken(0x02000002), null, "DeclaringType", 0);
			var type = new MetaSimpleResolvedType(module, token, declaringType, "TestType", 2);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(type.Module, Is.SameAs(module));
				Assert.That(type.Token, Is.EqualTo(token));
				Assert.That(type.DeclaringType, Is.SameAs(declaringType));
				Assert.That(type.Name, Is.EqualTo("TestType"));
				Assert.That(type.TypeArgs, Is.EqualTo(2));
			}
		}

		[Test]
		public void Apply_VisitorDispatchesCorrectly()
		{
			var type = new MetaSimpleResolvedType(WellKnownMetaModules.SomeModule, new MetaDataToken(0x02000001), null, "TestType", 0);
			var visitor = new DummyLogMetaTypeVisitor();

			type.Apply(visitor);

			using (Assert.EnterMultipleScope())
			{
				var record = visitor.Records.Single();
				Assert.That(record.Method.Name, Is.EqualTo(nameof(IMetaTypeVisitor.VisitSimpleResolved)));
				Assert.That(record.Type, Is.SameAs(type));
			}
		}

		[Test]
		public void ApplyWithArg_VisitorDispatchesAndReturnsCorrectly()
		{
			var type = new MetaSimpleResolvedType(WellKnownMetaModules.SomeModule, new MetaDataToken(0x02000001), null, "TestType", 0);
			var visitor = new DummyLogMetaTypeVisitor<string, int>(88);

			var result = type.Apply(visitor, "simple");
			Assert.That(result, Is.EqualTo(88), "Visitor return value should be propagated.");

			using (Assert.EnterMultipleScope())
			{
				var record = visitor.Records.Single();
				Assert.That(record.Method.Name, Is.EqualTo(nameof(IMetaTypeVisitor.VisitSimpleResolved)));
				Assert.That(record.Type, Is.SameAs(type));
				Assert.That(record.Argument, Is.EqualTo("simple"));
			}
		}
	}
}
