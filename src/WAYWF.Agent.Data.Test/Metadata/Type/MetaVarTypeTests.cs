// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Linq;
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test
{
	[TestFixture]
	public class MetaVarTypeTests
	{
		[Test]
		public void Constructor_SetsMethodAndIndex()
		{
			var type = new MetaVarType(true, 5);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(type.Method, Is.True);
				Assert.That(type.Index, Is.EqualTo(5));
			}
		}

		[Test]
		public void Apply_VisitorDispatchesCorrectly()
		{
			var type = new MetaVarType(true, 5);
			var visitor = new DummyLogMetaTypeVisitor();

			type.Apply(visitor);

			using (Assert.EnterMultipleScope())
			{
				var record = visitor.Records.Single();
				Assert.That(record.Method.Name, Is.EqualTo(nameof(IMetaTypeVisitor.VisitVar)));
				Assert.That(record.Type, Is.SameAs(type));
			}
		}

		[Test]
		public void ApplyWithArg_VisitorDispatchesAndReturnsCorrectly()
		{
			var type = new MetaVarType(true, 5);
			var visitor = new DummyLogMetaTypeVisitor<string, int>(202);

			var result = type.Apply(visitor, "var");
			Assert.That(result, Is.EqualTo(202), "Visitor return value should be propagated.");

			using (Assert.EnterMultipleScope())
			{
				var record = visitor.Records.Single();
				Assert.That(record.Method.Name, Is.EqualTo(nameof(IMetaTypeVisitor.VisitVar)));
				Assert.That(record.Type, Is.SameAs(type));
			}
		}
	}
}
