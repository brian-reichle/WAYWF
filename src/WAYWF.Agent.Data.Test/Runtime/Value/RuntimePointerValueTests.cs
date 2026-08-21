// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Linq;
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test;

[TestFixture]
public class RuntimePointerValueTests
{
	[Test]
	public void Constructor_StoresProperties()
	{
		var type = MetaKnownType.Int32;
		var address = new MemoryAddress(0x12345678);
		var innerValue = RuntimeNullValue.Instance;

		var pointerValue = new RuntimePointerValue(type, address, innerValue);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(pointerValue.Type, Is.SameAs(type));
			Assert.That(pointerValue.Address, Is.EqualTo(address));
			Assert.That(pointerValue.Value, Is.SameAs(innerValue));
		}
	}

	[Test]
	public void Apply_VisitorDispatchesCorrectly()
	{
		var pointerValue = new RuntimePointerValue(MetaKnownType.Int32, new MemoryAddress(0x1000), RuntimeNullValue.Instance);

		var visitor = new DummyLogValueVisitor();
		pointerValue.Apply(visitor);

		using (Assert.EnterMultipleScope())
		{
			var record = visitor.Records.Single();
			Assert.That(record.Value, Is.SameAs(pointerValue));
			Assert.That(record.IdentifiedType, Is.EqualTo(typeof(RuntimePointerValue)));
		}
	}
}
