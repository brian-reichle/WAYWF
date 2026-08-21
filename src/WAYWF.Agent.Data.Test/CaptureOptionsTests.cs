// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test;

[TestFixture]
public class CaptureOptionsTests
{
	[Test]
	public void Constructor_SetsProperties()
	{
		var options = new CaptureOptions(walkHeap: true, waitSeconds: 30);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(options.WalkHeap, Is.True);
			Assert.That(options.WaitSeconds, Is.EqualTo(30));
		}
	}

	[Test]
	public void Immutable_PropertiesRemainConstant()
	{
		var options = new CaptureOptions(walkHeap: false, waitSeconds: 5);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(options.WalkHeap, Is.False);
			Assert.That(options.WaitSeconds, Is.EqualTo(5));
		}
	}
}
