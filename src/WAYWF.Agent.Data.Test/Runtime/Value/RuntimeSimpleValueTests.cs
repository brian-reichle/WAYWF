// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Linq;
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test
{
	[TestFixture]
	public class RuntimeSimpleValueTests
	{
		[Test]
		public void Constructor_StoresProperties()
		{
			var id = Identity.NewSource().New();
			var type = MetaKnownType.Int32;
			var val = 42;

			var simpleValue = new RuntimeSimpleValue(id, type, val);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(simpleValue.ID, Is.EqualTo(id));
				Assert.That(simpleValue.Type, Is.SameAs(type));
				Assert.That(simpleValue.Value, Is.EqualTo(val));
				Assert.That(simpleValue.ToString(), Is.EqualTo(id.ToString()));
			}
		}

		[Test]
		public void Apply_VisitorDispatchesCorrectly()
		{
			var id = Identity.NewSource().New();
			var simpleValue = new RuntimeSimpleValue(id, MetaKnownType.Int32, 100);

			var visitor = new DummyLogValueVisitor();
			simpleValue.Apply(visitor);

			using (Assert.EnterMultipleScope())
			{
				var record = visitor.Records.Single();
				Assert.That(record.Value, Is.SameAs(simpleValue));
				Assert.That(record.IdentifiedType, Is.EqualTo(typeof(RuntimeSimpleValue)));
			}
		}
	}
}
