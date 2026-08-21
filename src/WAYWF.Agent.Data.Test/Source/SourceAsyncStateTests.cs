// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test;

[TestFixture]
public class SourceAsyncStateTests
{
	[Test]
	public void Constructor_SetsProperties()
	{
		var id = Identity.NewSource().New();
		var document = new SourceDocument(id, @"C:\src\Program.cs", SourceLanguage.CSharp, SourceDocumentType.Text);
		var sourceRef = new SourceRef(document, 10, 20, 1, 30);
		var yieldOffset = 0x42;

		var asyncState = new SourceAsyncState(yieldOffset, sourceRef);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(asyncState.YieldOffset, Is.EqualTo(yieldOffset));
			Assert.That(asyncState.YieldSource, Is.EqualTo(sourceRef));
		}
	}
}
