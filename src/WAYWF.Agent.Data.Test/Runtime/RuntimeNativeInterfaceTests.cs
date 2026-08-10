// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test
{
	[TestFixture]
	public class RuntimeNativeInterfaceTests
	{
		[Test]
		public void Constructor_SetsProperties()
		{
			var interfaceAddr = new RuntimeVirtualAddress(new MemoryAddress(0x1000));
			var vtblAddr = new RuntimeVirtualAddress(new MemoryAddress(0x2000));

			var nativeInterface = new RuntimeNativeInterface(interfaceAddr, vtblAddr);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(nativeInterface.InterfaceAddress, Is.EqualTo(interfaceAddr));
				Assert.That(nativeInterface.VtblAddress, Is.EqualTo(vtblAddr));
			}
		}
	}
}
