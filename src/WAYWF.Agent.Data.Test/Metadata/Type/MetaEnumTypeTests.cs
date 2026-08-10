// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Linq;
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test
{
	[TestFixture]
	public class MetaEnumTypeTests
	{
		[Test]
		public void Constructor_ThrowsOnNullUnderlyingType()
		{
			Assert.Throws<ArgumentNullException>(new Action(() =>
			{
				new MetaEnumType(
					WellKnownMetaModules.SomeModule,
					new MetaDataToken(0x02000001),
					null,
					"TestEnum",
					null,
					false,
					[],
					[]);
			}));
		}

		[Test]
		public void Constructor_ThrowsOnLengthMismatch()
		{
			var ex = Assert.Throws<ArgumentException>(new Action(() =>
			{
				new MetaEnumType(
					WellKnownMetaModules.SomeModule,
					new MetaDataToken(0x02000001),
					null,
					"TestEnum",
					MetaKnownType.Int32,
					false,
					["A"],
					[]);
			}));
			Assert.That(ex.Message, Is.EqualTo("length mismatch."));
		}

		[Test]
		public void Constructor_SetsProperties()
		{
			var module = WellKnownMetaModules.SomeModule;
			var token = new MetaDataToken(0x02000001);

			var enumType = new MetaEnumType(
				module,
				token,
				null,
				"TestEnum",
				MetaKnownType.Int32,
				false,
				["A", "B"],
				[1UL, 2UL]);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(enumType.Module, Is.SameAs(module));
				Assert.That(enumType.Token, Is.EqualTo(token));
				Assert.That(enumType.DeclaringType, Is.Null);
				Assert.That(enumType.Name, Is.EqualTo("TestEnum"));
				Assert.That(enumType.UnderlyingType, Is.SameAs(MetaKnownType.Int32));
				Assert.That(enumType.TypeArgs, Is.Zero);
			}
		}

		[Test]
		public void Format_RegularEnum_MatchesValue()
		{
			var enumType = new MetaEnumType(
				WellKnownMetaModules.SomeModule,
				new MetaDataToken(0x02000001),
				null,
				"TestEnum",
				MetaKnownType.Int32,
				false,
				["Zero", "One", "Two"],
				[0UL, 1UL, 2UL]);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(enumType.Format(1), Is.EqualTo("One"));
				Assert.That(enumType.Format(3), Is.EqualTo("3"));
			}
		}

		[Test]
		public void Format_FlagsEnum_ZeroValue()
		{
			var enumType = new MetaEnumType(
				WellKnownMetaModules.SomeModule,
				new MetaDataToken(0x02000001),
				null,
				"TestEnum",
				MetaKnownType.Int32,
				true,
				["None", "A"],
				[0UL, 1UL]);

			Assert.That(enumType.Format(0), Is.EqualTo("None"));
		}

		[Test]
		public void Format_FlagsEnum_ExactMatchesAndCombinations()
		{
			var enumType = new MetaEnumType(
				WellKnownMetaModules.SomeModule,
				new MetaDataToken(0x02000001),
				null,
				"TestEnum",
				MetaKnownType.Int32,
				true,
				["A", "B", "C"],
				[1UL, 2UL, 4UL]);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(enumType.Format(1), Is.EqualTo("A"));
				Assert.That(enumType.Format(3), Is.EqualTo("B|A"));
				Assert.That(enumType.Format(7), Is.EqualTo("C|B|A"));
			}
		}

		[Test]
		public void Format_FlagsEnum_WithResidue()
		{
			var enumType = new MetaEnumType(
				WellKnownMetaModules.SomeModule,
				new MetaDataToken(0x02000001),
				null,
				"TestEnum",
				MetaKnownType.Int32,
				true,
				["A", "B"],
				[1UL, 2UL]);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(enumType.Format(5), Is.EqualTo("A|4"));
				Assert.That(enumType.Format(8), Is.EqualTo("8"));
			}
		}

		[Test]
		public void Apply_VisitorDispatchesCorrectly()
		{
			var enumType = new MetaEnumType(
				WellKnownMetaModules.SomeModule,
				new MetaDataToken(0x02000001),
				null,
				"TestEnum",
				MetaKnownType.Int32,
				false,
				[],
				[]);

			var visitor = new DummyLogMetaTypeVisitor();

			enumType.Apply(visitor);

			using (Assert.EnterMultipleScope())
			{
				var record = visitor.Records.Single();
				Assert.That(record.Method.Name, Is.EqualTo(nameof(IMetaTypeVisitor.VisitEnum)));
				Assert.That(record.Type, Is.SameAs(enumType));
			}
		}

		[Test]
		public void ApplyWithArg_VisitorDispatchesAndReturnsCorrectly()
		{
			var enumType = new MetaEnumType(
				WellKnownMetaModules.SomeModule,
				new MetaDataToken(0x02000001),
				null,
				"TestEnum",
				MetaKnownType.Int32,
				false,
				[],
				[]);

			var visitor = new DummyLogMetaTypeVisitor<string, int>(100);

			var result = enumType.Apply(visitor, "arg");
			Assert.That(result, Is.EqualTo(100), "Visitor return value should be propagated.");

			using (Assert.EnterMultipleScope())
			{
				var record = visitor.Records.Single();
				Assert.That(record.Method.Name, Is.EqualTo(nameof(IMetaTypeVisitor.VisitEnum)));
				Assert.That(record.Type, Is.SameAs(enumType));
				Assert.That(record.Argument, Is.EqualTo("arg"));
			}
		}
	}
}
