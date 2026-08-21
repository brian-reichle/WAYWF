// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test;

[TestFixture]
public class MetaFieldTests
{
	[Test]
	public void Constructor_StoresPropertiesVerbatim()
	{
		var token = new MetaDataToken(0x04000001);
		var type = MetaKnownType.String;
		var field = new MetaField(
			token: token,
			type: type,
			name: "_myField");

		using (Assert.EnterMultipleScope())
		{
			Assert.That(field.Token, Is.EqualTo(token));
			Assert.That(field.Type, Is.SameAs(type));
			Assert.That(field.Name, Is.EqualTo("_myField"));
		}
	}
}
