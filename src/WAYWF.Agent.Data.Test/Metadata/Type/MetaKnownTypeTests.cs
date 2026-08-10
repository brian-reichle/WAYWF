// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test
{
	[TestFixture]
	public class MetaKnownTypeTests
	{
		[Test]
		public void FromFullName_ValidName_ReturnsCorrectInstance()
		{
			foreach (var field in typeof(MetaKnownType).GetFields(BindingFlags.Public | BindingFlags.Static))
			{
				var type = (MetaKnownType)field.GetValue(null);
				Assert.That(MetaKnownType.FromFullName(type.Name), Is.SameAs(type));
			}
		}

		[Test]
		public void FromFullName_InvalidName_ReturnsNull()
		{
			Assert.That(MetaKnownType.FromFullName("System.InvalidTypeName"), Is.Null);
		}

		[Test]
		public void CodeProperty()
		{
			foreach (var field in typeof(MetaKnownType).GetFields(BindingFlags.Public | BindingFlags.Static))
			{
				var type = (MetaKnownType)field.GetValue(null);

				Assert.That(type.Code.ToString(), Is.EqualTo(field.Name));
			}
		}

		[Test]
		public void NameProperty()
		{
			foreach (var field in typeof(MetaKnownType).GetFields(BindingFlags.Public | BindingFlags.Static))
			{
				var type = (MetaKnownType)field.GetValue(null);
				var clrType = typeof(object).Assembly.GetType(type.Name, throwOnError: true);
				Assert.That(clrType.Name, Is.EqualTo(field.Name));
			}
		}

		[Test]
		public void Properties_MatchExpectedValues()
		{
			using (Assert.EnterMultipleScope())
			{
				Assert.That(MetaKnownType.Boolean.Size, Is.EqualTo(sizeof(bool)));
				Assert.That(MetaKnownType.Byte.Size, Is.EqualTo(sizeof(byte)));
				Assert.That(MetaKnownType.Int16.Size, Is.EqualTo(sizeof(short)));
				Assert.That(MetaKnownType.Int32.Size, Is.EqualTo(sizeof(int)));
				Assert.That(MetaKnownType.Int64.Size, Is.EqualTo(sizeof(long)));
				Assert.That(MetaKnownType.Void.Size, Is.Zero);
			}
		}

		[Test]
		public void Apply_VisitorDispatchesCorrectly()
		{
			var knownType = MetaKnownType.Int32;
			var visitor = new DummyLogMetaTypeVisitor();

			knownType.Apply(visitor);

			using (Assert.EnterMultipleScope())
			{
				var record = visitor.Records.Single();
				Assert.That(record.Method.Name, Is.EqualTo(nameof(IMetaTypeVisitor.VisitKnownType)));
				Assert.That(record.Type, Is.SameAs(knownType));
			}
		}

		[Test]
		public void ApplyWithArg_VisitorDispatchesAndReturnsCorrectly()
		{
			var knownType = MetaKnownType.Int32;
			var visitor = new DummyLogMetaTypeVisitor<string, int>(101);

			var result = knownType.Apply(visitor, "known");
			Assert.That(result, Is.EqualTo(101), "Visitor return value should be propagated.");

			using (Assert.EnterMultipleScope())
			{
				var record = visitor.Records.Single();
				Assert.That(record.Method.Name, Is.EqualTo(nameof(IMetaTypeVisitor.VisitKnownType)));
				Assert.That(record.Type, Is.SameAs(knownType));
				Assert.That(record.Argument, Is.EqualTo("known"));
			}
		}
	}
}
