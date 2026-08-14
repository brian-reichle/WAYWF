// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;
using System.Reflection;
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test
{
	[TestFixture]
	public class MetaMethodTests
	{
		[Test]
		public void Constructor_StoresPropertiesVerbatim()
		{
			var token = new MetaDataToken(0x06000001);
			var module = WellKnownMetaModules.SomeModule;
			var declaringType = new MetaSimpleResolvedType(module, new MetaDataToken(0x02000001), null, "MyClass", 0);
			var signature = new MetaMethodSignature(
				CallingConventions.Standard,
				0,
				new MetaVariable(MetaKnownType.Void, null, false, false),
				[]);
			var local1 = new MetaVariable(MetaKnownType.Int32, "tmp", false, false);
			var locals = ImmutableArray.Create(local1);

			var method = new MetaMethod(
				token: token,
				module: module,
				declaringType: declaringType,
				name: "MyMethod",
				signature: signature,
				locals: locals);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(method.Token, Is.EqualTo(token));
				Assert.That(method.Module, Is.SameAs(module));
				Assert.That(method.DeclaringType, Is.SameAs(declaringType));
				Assert.That(method.Name, Is.EqualTo("MyMethod"));
				Assert.That(method.Signature, Is.SameAs(signature));
				Assert.That(method.Locals, Is.EqualTo(locals));
			}
		}
	}
}
