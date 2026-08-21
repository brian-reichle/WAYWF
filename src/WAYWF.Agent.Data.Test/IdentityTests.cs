// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test;

[TestFixture]
public class IdentityTests
{
	[Test]
	public void NewIdentity_HasNonZeroId()
	{
		var source = Identity.NewSource();
		var identity = source.New();
		Assert.That(identity.ID, Is.Not.Zero, "Identity should have a non-zero ID after first access.");
	}

	[Test]
	public void MultipleIdentities_HaveDistinctIds()
	{
		var source = Identity.NewSource();
		var id1 = source.New();
		var id2 = source.New();

		// Access IDs to ensure they are generated
		var v1 = id1.ID;
		var v2 = id2.ID;
		Assert.That(v2, Is.GreaterThan(v1), "Subsequent IDs should be greater than previous ones.");
	}
}
