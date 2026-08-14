// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test
{
	[TestFixture]
	public class MemoryAddressTests
	{
		[Test]
		public void Equality_OperatorWorks()
		{
			var a = new MemoryAddress(0x1234);
			var b = new MemoryAddress(0x1234);
			var c = new MemoryAddress(0x5678);

			using (Assert.EnterMultipleScope())
			{
				// We are specifically trying to test the operators here.
#pragma warning disable NUnit2010 // Use EqualConstraint for better assertion messages in case of failure
				Assert.That(a == b, Is.True);
				Assert.That(a == c, Is.False);
				Assert.That(a != c, Is.True);
				Assert.That(a != b, Is.False);
#pragma warning restore NUnit2010 // Use EqualConstraint for better assertion messages in case of failure
			}
		}

		[Test]
		public void Equality()
		{
			var addr1 = new MemoryAddress(0x1234);
			var addr2 = new MemoryAddress(0x1234);
			var addr3 = new MemoryAddress(0x5678);

			Assert.That(addr1, Is.EqualTo(addr2));
			Assert.That(addr1, Is.Not.EqualTo(addr3));
		}

		[Test]
		public void ToString_ReturnsHexadecimal16Digits()
		{
			var addr = new MemoryAddress(0x1AB);

			// Expected 16‑digit hex with leading zeros
			Assert.That(addr.ToString(), Is.EqualTo("00000000000001AB"));
		}

		[Test]
		public void IsNull_IsTrueForZeroAddress()
		{
			var zero = new MemoryAddress(0);
			Assert.That(zero.IsNull, Is.True);

			var nonZero = new MemoryAddress(1);
			Assert.That(nonZero.IsNull, Is.False);
		}
	}
}
