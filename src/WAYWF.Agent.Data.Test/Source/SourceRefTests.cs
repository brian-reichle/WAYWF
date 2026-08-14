// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test
{
	[TestFixture]
	public class SourceRefTests
	{
		[Test]
		public void Constructor_SetsProperties()
		{
			var id = Identity.NewSource().New();
			var document = new SourceDocument(id, @"C:\src\Program.cs", SourceLanguage.CSharp, SourceDocumentType.Text);
			var fromLine = 10;
			var toLine = 20;
			var fromColumn = 1;
			var toColumn = 30;

			var sourceRef = new SourceRef(document, fromLine, toLine, fromColumn, toColumn);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(sourceRef.Document, Is.EqualTo(document));
				Assert.That(sourceRef.FromLine, Is.EqualTo(fromLine));
				Assert.That(sourceRef.ToLine, Is.EqualTo(toLine));
				Assert.That(sourceRef.FromColumn, Is.EqualTo(fromColumn));
				Assert.That(sourceRef.ToColumn, Is.EqualTo(toColumn));
			}
		}
	}
}
