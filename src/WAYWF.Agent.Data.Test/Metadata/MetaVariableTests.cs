// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test;

[TestFixture]
public class MetaVariableTests
{
	[Test]
	public void Constructor_StoresPropertiesVerbatim()
	{
		var type = MetaKnownType.Int32;
		var variable = new MetaVariable(
			type: type,
			name: "localVar",
			isByRef: true,
			pinned: false);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(variable.Type, Is.SameAs(type));
			Assert.That(variable.Name, Is.EqualTo("localVar"));
			Assert.That(variable.IsByRef, Is.True);
			Assert.That(variable.Pinned, Is.False);
		}
	}
}
