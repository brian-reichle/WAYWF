// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;
using System.Linq;
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test
{
	[TestFixture]
	public class RuntimeRcwValueTests
	{
		[Test]
		public void Constructor_StoresProperties()
		{
			var id = Identity.NewSource().New();
			var type = MetaKnownType.Object;
			var interfaceTypes = ImmutableArray.Create<MetaTypeBase>(MetaKnownType.String);
			var rva1 = new RuntimeVirtualAddress(new MemoryAddress(0x1000));
			var rva2 = new RuntimeVirtualAddress(new MemoryAddress(0x2000));
			var nativeInterface = new RuntimeNativeInterface(rva1, rva2);
			var interfacePointers = ImmutableArray.Create(nativeInterface);

			var rcwValue = new RuntimeRcwValue(id, type, interfaceTypes, interfacePointers);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(rcwValue.ID, Is.EqualTo(id));
				Assert.That(rcwValue.Type, Is.SameAs(type));
				Assert.That(rcwValue.InterfaceTypes, Is.EqualTo(interfaceTypes));
				Assert.That(rcwValue.InterfacePointers, Is.EqualTo(interfacePointers));
				Assert.That(rcwValue.ToString(), Is.EqualTo(id.ToString()));
			}
		}

		[Test]
		public void Apply_VisitorDispatchesCorrectly()
		{
			var id = Identity.NewSource().New();
			var rcwValue = new RuntimeRcwValue(id, MetaKnownType.Object, [], []);

			var visitor = new DummyLogValueVisitor();
			rcwValue.Apply(visitor);

			using (Assert.EnterMultipleScope())
			{
				var record = visitor.Records.Single();
				Assert.That(record.Value, Is.SameAs(rcwValue));
				Assert.That(record.IdentifiedType, Is.EqualTo(typeof(RuntimeRcwValue)));
			}
		}
	}
}
