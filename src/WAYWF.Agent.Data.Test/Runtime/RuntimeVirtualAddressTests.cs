// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test;

[TestFixture]
public class RuntimeVirtualAddressTests
{
	[Test]
	public void Constructor_AddressOnly_SetsDefaults()
	{
		var address = new MemoryAddress(0x1234);
		var rva = new RuntimeVirtualAddress(address);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(rva.Address, Is.EqualTo(address));
			Assert.That(rva.Image, Is.Null);
			Assert.That(rva.Offset, Is.Zero);
		}
	}

	[Test]
	public void Constructor_AllArguments_SetsProperties()
	{
		var address = new MemoryAddress(0x1234);
		var rva = new RuntimeVirtualAddress(address, "foo.dll", 0x50);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(rva.Address, Is.EqualTo(address));
			Assert.That(rva.Image, Is.EqualTo("foo.dll"));
			Assert.That(rva.Offset, Is.EqualTo(0x50));
		}
	}

	[Test]
	public void ToString_NullImage_ReturnsAddressString()
	{
		var address = new MemoryAddress(0x1234);
		var rva = new RuntimeVirtualAddress(address);

		Assert.That(rva.ToString(), Is.EqualTo("0000000000001234"));
	}

	[Test]
	public void ToString_WithImage_ReturnsImageAndOffset()
	{
		var address = new MemoryAddress(0x1234);
		var rva = new RuntimeVirtualAddress(address, "foo.dll", 100);

		Assert.That(rva.ToString(), Is.EqualTo("foo.dll+100"));
	}
}
