// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;
using System.Linq;
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test
{
	[TestFixture]
	public class RuntimeILFrameTests
	{
		[Test]
		public void Constructor_NullMethod_ThrowsArgumentNullException()
		{
			Assert.That(
				() => new RuntimeILFrame(
					method: null,
					ilOffset: 0,
					ilMapping: RuntimeILMapping.Exact,
					source: null,
					@this: null,
					typeArgs: default,
					arguments: default,
					locals: default,
					localNames: default),
				Throws.ArgumentNullException.With.Property("ParamName").EqualTo("method"));
		}

		[Test]
		public void Constructor_WithDeclaringType_SetsStartOfMethodArgsFromDeclaringType()
		{
			var declaringType = new MetaSimpleResolvedType(
				WellKnownMetaModules.SomeModule,
				new MetaDataToken(0x02000001),
				declaringType: null,
				name: "DeclaringType",
				typeArgs: 3);

			var method = new MetaMethod(
				new MetaDataToken(0x06000001),
				WellKnownMetaModules.SomeModule,
				declaringType,
				"TestMethod",
				signature: null,
				locals: default);

			var frame = new RuntimeILFrame(
				method: method,
				ilOffset: 0,
				ilMapping: RuntimeILMapping.Exact,
				source: null,
				@this: null,
				typeArgs: default,
				arguments: default,
				locals: default,
				localNames: default);

			Assert.That(frame.StartOfMethodArgs, Is.EqualTo(3));
		}

		[Test]
		public void Constructor_NullDeclaringType_StartOfMethodArgsIsZero()
		{
			var method = new MetaMethod(
				new MetaDataToken(0x06000001),
				WellKnownMetaModules.SomeModule,
				declaringType: null,
				name: "GlobalMethod",
				signature: null,
				locals: default);

			var frame = new RuntimeILFrame(
				method: method,
				ilOffset: 0,
				ilMapping: RuntimeILMapping.Exact,
				source: null,
				@this: null,
				typeArgs: default,
				arguments: default,
				locals: default,
				localNames: default);

			Assert.That(frame.StartOfMethodArgs, Is.Zero);
		}

		[Test]
		public void Constructor_StoresPropertiesVerbatim()
		{
			var declaringType = new MetaSimpleResolvedType(
				WellKnownMetaModules.SomeModule,
				new MetaDataToken(0x02000001),
				declaringType: null,
				name: "DeclaringType",
				typeArgs: 2);

			var method = new MetaMethod(
				new MetaDataToken(0x06000001),
				WellKnownMetaModules.SomeModule,
				declaringType,
				"TestMethod",
				signature: null,
				locals: default);

			var sourceDoc = new SourceDocument(Identity.NewSource().New(), @"C:\src\Test.cs", SourceLanguage.CSharp, SourceDocumentType.Text);
			var source = new SourceRef(sourceDoc, 10, 20, 1, 50);
			var @this = RuntimeNullValue.Instance;
			var typeArg = MetaKnownType.Object;
			var typeArgs = ImmutableArray.Create<MetaTypeBase>(typeArg);
			var arguments = ImmutableArray.Create(RuntimeNullValue.Instance);
			var locals = ImmutableArray.Create(RuntimeNullValue.Instance);
			var localNames = ImmutableArray.Create("loc1");

			var frame = new RuntimeILFrame(
				method: method,
				ilOffset: 42,
				ilMapping: RuntimeILMapping.Approximate,
				source: source,
				@this: @this,
				typeArgs: typeArgs,
				arguments: arguments,
				locals: locals,
				localNames: localNames);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(frame.Method, Is.SameAs(method));
				Assert.That(frame.ILOffset, Is.EqualTo(42));
				Assert.That(frame.ILMapping, Is.EqualTo(RuntimeILMapping.Approximate));
				Assert.That(frame.Source, Is.SameAs(source));
				Assert.That(frame.This, Is.SameAs(@this));
				Assert.That(frame.TypeArgs, Is.EqualTo(typeArgs));
				Assert.That(frame.Arguments, Is.EqualTo(arguments));
				Assert.That(frame.Locals, Is.EqualTo(locals));
				Assert.That(frame.LocalNames, Is.EqualTo(localNames));
				Assert.That(frame.StartOfMethodArgs, Is.EqualTo(2));
			}
		}

		[Test]
		public void Duration_GetAndSet()
		{
			var method = new MetaMethod(
				new MetaDataToken(0x06000001),
				WellKnownMetaModules.SomeModule,
				declaringType: null,
				name: "TestMethod",
				signature: null,
				locals: default);

			var frame = new RuntimeILFrame(
				method: method,
				ilOffset: 0,
				ilMapping: RuntimeILMapping.Exact,
				source: null,
				@this: null,
				typeArgs: default,
				arguments: default,
				locals: default,
				localNames: default);

			Assert.That(frame.Duration, Is.Null);

			frame.Duration = 123.45;
			Assert.That(frame.Duration, Is.EqualTo(123.45));
		}

		[Test]
		public void Apply_VisitorDispatchesCorrectly()
		{
			var method = new MetaMethod(
				new MetaDataToken(0x06000001),
				WellKnownMetaModules.SomeModule,
				declaringType: null,
				name: "TestMethod",
				signature: null,
				locals: default);

			var frame = new RuntimeILFrame(
				method: method,
				ilOffset: 0,
				ilMapping: RuntimeILMapping.Exact,
				source: null,
				@this: null,
				typeArgs: default,
				arguments: default,
				locals: default,
				localNames: default);

			var visitor = new DummyLogFrameVisitor();
			frame.Apply(visitor);

			using (Assert.EnterMultipleScope())
			{
				var record = visitor.Records.Single();
				Assert.That(record.Frame, Is.SameAs(frame));
				Assert.That(record.IdentifiedType, Is.EqualTo(typeof(RuntimeILFrame)));
			}
		}
	}
}
