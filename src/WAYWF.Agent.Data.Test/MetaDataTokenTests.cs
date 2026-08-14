// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test
{
	[TestFixture]
	public class MetaDataTokenTests
	{
		[Test]
		public void Nil_IsNil()
		{
			Assert.That(MetaDataToken.Nil.IsNil, Is.True);
		}

		[Test]
		public void Nil_HasModuleTokenType()
		{
			Assert.That(MetaDataToken.Nil.TokenType, Is.EqualTo(TokenType.Module));
		}

		[Test]
		public void Module_IsNil()
		{
			// Row index 1 means the low 24 bits are non-zero — not nil.
			Assert.That(MetaDataToken.Module.IsNil, Is.False);
		}

		[Test]
		public void Module_HasModuleTokenType()
		{
			Assert.That(MetaDataToken.Module.TokenType, Is.EqualTo(TokenType.Module));
		}

		[Test]
		public void TokenType_ExtractsHighByte()
		{
			var token = new MetaDataToken(unchecked((int)0x02000042));
			Assert.That(token.TokenType, Is.EqualTo(TokenType.TypeDef));
		}

		[Test]
		public void TokenType_IsUnaffectedByLowBytes()
		{
			var a = new MetaDataToken(unchecked((int)0x04000001));
			var b = new MetaDataToken(unchecked((int)0x04FFFFFF));
			Assert.That(a.TokenType, Is.EqualTo(b.TokenType));
		}

		[TestCase(0x00000000, ExpectedResult = true)] // Nil sentinel
		[TestCase(0x02000000, ExpectedResult = true)] // Non-nil type prefix with zero row
		[TestCase(0x02000001, ExpectedResult = false)] // Non-zero row index
		[TestCase(0x00000001, ExpectedResult = false)] // Module type, row 1
		public bool IsNil_DetectsLow24Bits(int tokenId)
		{
			var token = new MetaDataToken(tokenId);
			return token.IsNil;
		}

		[Test]
		public void ToString_ReturnsEightDigitUppercaseHex()
		{
			var token = new MetaDataToken(0x0600001A);
			Assert.That(token.ToString(), Is.EqualTo("0600001A"));
		}

		[Test]
		public void ToString_PadsSmallValuesToEightDigits()
		{
			var token = new MetaDataToken(0x00000001);
			Assert.That(token.ToString(), Is.EqualTo("00000001"));
		}

		[Test]
		public void ToString_Nil_IsAllZeros()
		{
			Assert.That(MetaDataToken.Nil.ToString(), Is.EqualTo("00000000"));
		}

#pragma warning disable NUnit2010 // Use EqualConstraint for better assertion messages in case of failure
		[Test]
		public void EqualityOperator_SameId_ReturnsTrue()
		{
			var a = new MetaDataToken(0x02000003);
			var b = new MetaDataToken(0x02000003);
			Assert.That(a == b, Is.True);
		}

		[Test]
		public void EqualityOperator_DifferentId_ReturnsFalse()
		{
			var a = new MetaDataToken(0x02000003);
			var b = new MetaDataToken(0x02000004);
			Assert.That(a == b, Is.False);
		}

		[Test]
		public void InequalityOperator_DifferentId_ReturnsTrue()
		{
			var a = new MetaDataToken(0x02000003);
			var b = new MetaDataToken(0x02000004);
			Assert.That(a != b, Is.True);
		}

		[Test]
		public void InequalityOperator_SameId_ReturnsFalse()
		{
			var a = new MetaDataToken(0x02000003);
			var b = new MetaDataToken(0x02000003);
			Assert.That(a != b, Is.False);
		}

		[Test]
		public void Equals_BoxedValue_SameId_ReturnsTrue()
		{
			var a = new MetaDataToken(0x02000003);
			object b = new MetaDataToken(0x02000003);
			Assert.That(a.Equals(b), Is.True);
		}

		[Test]
		public void Equals_BoxedValue_DifferentId_ReturnsFalse()
		{
			var a = new MetaDataToken(0x02000003);
			object b = new MetaDataToken(0x02000004);
			Assert.That(a.Equals(b), Is.False);
		}

		[Test]
		public void Equals_BoxedValue_WrongType_ReturnsFalse()
		{
			var a = new MetaDataToken(0x02000003);
			Assert.That(a.Equals("not a token"), Is.False);
		}
#pragma warning restore NUnit2010 // Use EqualConstraint for better assertion messages in case of failure

		[Test]
		public void GetHashCode_EqualTokens_HaveSameHashCode()
		{
			var a = new MetaDataToken(0x02000003);
			var b = new MetaDataToken(0x02000003);
			Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
		}
	}
}
