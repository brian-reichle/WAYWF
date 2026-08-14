// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Linq;
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test
{
	[TestFixture]
	public class RuntimeInternalFrameTests
	{
		[Test]
		public void Constructor_StoresProperties()
		{
			var frame = new RuntimeInternalFrame(RuntimeInternalFrameKind.InternalCall);
			Assert.That(frame.InternalFrameType, Is.EqualTo(RuntimeInternalFrameKind.InternalCall));
		}

		[Test]
		public void Apply_VisitorDispatchesCorrectly()
		{
			var frame = new RuntimeInternalFrame(RuntimeInternalFrameKind.InternalCall);

			var visitor = new DummyLogFrameVisitor();
			frame.Apply(visitor);

			using (Assert.EnterMultipleScope())
			{
				var record = visitor.Records.Single();
				Assert.That(record.Frame, Is.SameAs(frame));
				Assert.That(record.IdentifiedType, Is.EqualTo(typeof(RuntimeInternalFrame)));
			}
		}
	}
}
