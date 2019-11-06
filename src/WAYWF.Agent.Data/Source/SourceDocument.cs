// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace WAYWF.Agent.Data
{
	public sealed class SourceDocument
	{
		public SourceDocument(Identity id, string path, SourceLanguage language, SourceDocumentType documentType)
		{
			ID = id;
			Path = path;
			Language = language;
			DocumentType = documentType;
		}

		public Identity ID { get; }
		public string Path { get; }
		public SourceLanguage Language { get; }
		public SourceDocumentType DocumentType { get; }
	}
}
