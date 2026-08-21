// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test;

[TestFixture]
public class RuntimeWindowTests
{
	[Test]
	public void Constructor_SetsProperties()
	{
		var threadId = 100;
		var handle = new IntPtr(0x123456);
		var owner = new IntPtr(0x654321);
		var caption = "Main Window";
		var className = "Form";
		var isVisible = true;
		var isEnabled = false;

		var window = new RuntimeWindow(threadId, handle, owner, caption, className, isVisible, isEnabled);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(window.ThreadID, Is.EqualTo(threadId));
			Assert.That(window.Handle, Is.EqualTo(handle));
			Assert.That(window.Owner, Is.EqualTo(owner));
			Assert.That(window.Caption, Is.EqualTo(caption));
			Assert.That(window.ClassName, Is.EqualTo(className));
			Assert.That(window.IsVisible, Is.True);
			Assert.That(window.IsEnabled, Is.False);
		}
	}
}
