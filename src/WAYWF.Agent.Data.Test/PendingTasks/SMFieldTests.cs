// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test
{
	[TestFixture]
	public class SMFieldTests
	{
		[Test]
		public void Constructor_SetsProperties()
		{
			var token = new MetaDataToken(0x04000001);
			var name = "stateMachineField";

			var field = new SMField(token, name);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(field.FieldToken, Is.EqualTo(token));
				Assert.That(field.Name, Is.EqualTo(name));
			}
		}
	}
}
