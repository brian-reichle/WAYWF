// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;
using System.Linq;
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test;

[TestFixture]
public class MetaGenTypeTests
{
	[Test]
	public void Constructor_SetsBaseTypeAndTypeArgs()
	{
		var baseType = new MetaSimpleResolvedType(WellKnownMetaModules.SomeModule, new MetaDataToken(0x02000001), null, "BaseType", 1);
		var typeArgs = ImmutableArray.Create<MetaTypeBase>(MetaKnownType.Int32);
		var genType = new MetaGenType(baseType, typeArgs);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(genType.BaseType, Is.SameAs(baseType));
			Assert.That(genType.TypeArgs, Is.EqualTo(typeArgs));
		}
	}

	[Test]
	public void Apply_VisitorDispatchesCorrectly()
	{
		var baseType = new MetaSimpleResolvedType(WellKnownMetaModules.SomeModule, new MetaDataToken(0x02000001), null, "BaseType", 1);
		var genType = new MetaGenType(baseType, []);
		var visitor = new DummyLogMetaTypeVisitor();

		genType.Apply(visitor);

		using (Assert.EnterMultipleScope())
		{
			var record = visitor.Records.Single();
			Assert.That(record.Method.Name, Is.EqualTo(nameof(IMetaTypeVisitor.VisitGen)));
			Assert.That(record.Type, Is.SameAs(genType));
		}
	}

	[Test]
	public void ApplyWithArg_VisitorDispatchesAndReturnsCorrectly()
	{
		var baseType = new MetaSimpleResolvedType(WellKnownMetaModules.SomeModule, new MetaDataToken(0x02000001), null, "BaseType", 1);
		var genType = new MetaGenType(baseType, []);

		var visitor = new DummyLogMetaTypeVisitor<string, int>(123);

		var result = genType.Apply(visitor, "gen");
		Assert.That(result, Is.EqualTo(123), "Visitor return value should be propagated.");

		using (Assert.EnterMultipleScope())
		{
			var record = visitor.Records.Single();
			Assert.That(record.Method.Name, Is.EqualTo(nameof(IMetaTypeVisitor.VisitGen)));
			Assert.That(record.Type, Is.SameAs(genType));
			Assert.That(record.Argument, Is.EqualTo("gen"));
		}
	}
}
