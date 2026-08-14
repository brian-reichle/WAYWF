// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Linq;
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test
{
	[TestFixture]
	public class RuntimeNullValueTests
	{
		[Test]
		public void Instance_NotNull()
		{
			Assert.That(RuntimeNullValue.Instance, Is.Not.Null);
		}

		[Test]
		public void Instance_ReturnsSameObject()
		{
			var first = RuntimeNullValue.Instance;
			var second = RuntimeNullValue.Instance;
			Assert.That(first, Is.SameAs(second));
		}

		[Test]
		public void ReferenceCount_DefaultIsOne()
		{
			Assert.That(RuntimeNullValue.Instance.ReferenceCount, Is.EqualTo(1));
		}

		[Test]
		public void Apply_VisitorDispatchesCorrectly()
		{
			var visitor = new DummyLogValueVisitor();
			RuntimeNullValue.Instance.Apply(visitor);

			using (Assert.EnterMultipleScope())
			{
				var record = visitor.Records.Single();
				Assert.That(record.Value, Is.SameAs(RuntimeNullValue.Instance));
				Assert.That(record.IdentifiedType, Is.EqualTo(typeof(RuntimeNullValue)));
			}
		}
	}
}
