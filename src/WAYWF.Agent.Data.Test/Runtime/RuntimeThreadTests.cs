// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test
{
	[TestFixture]
	public class RuntimeThreadTests
	{
		[Test]
		public void Constructor_SetsProperties()
		{
			var threadId = 42;
			var userState = RuntimeThreadStates.Background;
			var chains = ImmutableArray<RuntimeFrameChain>.Empty;
			var blockingObjects = ImmutableArray<RuntimeBlockingObject>.Empty;

			var thread = new RuntimeThread(threadId, userState, chains, blockingObjects);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(thread.ThreadID, Is.EqualTo(threadId));
				Assert.That(thread.UserState, Is.EqualTo(userState));
				Assert.That(thread.Chains, Is.EqualTo(chains));
				Assert.That(thread.BlockingObject, Is.EqualTo(blockingObjects));
			}
		}
	}
}
