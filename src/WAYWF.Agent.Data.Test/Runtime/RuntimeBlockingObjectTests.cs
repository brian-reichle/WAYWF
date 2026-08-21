// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test;

[TestFixture]
public class RuntimeBlockingObjectTests
{
	[Test]
	public void Constructor_SetsProperties()
	{
		var value = RuntimeNullValue.Instance;
		var ownerId = 42;
		var timeout = 1000;
		var blockingReason = RuntimeBlockingReason.Wait;

		var blockingObject = new RuntimeBlockingObject(value, ownerId, timeout, blockingReason);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(blockingObject.Value, Is.EqualTo(value));
			Assert.That(blockingObject.OwnerId, Is.EqualTo(ownerId));
			Assert.That(blockingObject.Timeout, Is.EqualTo(timeout));
			Assert.That(blockingObject.BlockingReason, Is.EqualTo(blockingReason));
		}
	}
}
