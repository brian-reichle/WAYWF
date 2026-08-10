// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test
{
	[TestFixture]
	public class SourceDocumentTests
	{
		[Test]
		public void Constructor_SetsProperties()
		{
			var id = Identity.NewSource().New();
			var path = @"C:\src\Program.cs";
			var language = SourceLanguage.CSharp;
			var documentType = SourceDocumentType.Text;

			var document = new SourceDocument(id, path, language, documentType);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(document.ID, Is.EqualTo(id));
				Assert.That(document.Path, Is.EqualTo(path));
				Assert.That(document.Language, Is.EqualTo(language));
				Assert.That(document.DocumentType, Is.EqualTo(documentType));
			}
		}
	}
}
