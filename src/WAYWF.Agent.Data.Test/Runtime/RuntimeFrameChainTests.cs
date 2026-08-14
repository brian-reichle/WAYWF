// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test
{
	[TestFixture]
	public class RuntimeFrameChainTests
	{
		[Test]
		public void Constructor_SetsProperties()
		{
			var reason = RuntimeFrameChainReason.ExceptionFilter;
			var frames = ImmutableArray<RuntimeFrame>.Empty;

			var chain = new RuntimeFrameChain(reason, frames);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(chain.Reason, Is.EqualTo(reason));
				Assert.That(chain.Frames, Is.EqualTo(frames));
			}
		}
	}
}
