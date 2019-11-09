// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.IO;

namespace WAYWF.UI.VirtualFile
{
	sealed class HtmlVirtualFile : VirtualFileBase
	{
		public HtmlVirtualFile(string baseFileName, string xmlContent)
			: base(baseFileName + ".xhtml")
		{
			_xmlContent = xmlContent;
		}

		public override string Extension => ".xhtml";

		public override byte[] GenerateContent()
		{
			using var writeStream = new MemoryStream();
			HtmlTranslator.Transform(_xmlContent, writeStream);
			return writeStream.ToArray();
		}

		readonly string _xmlContent;
	}
}
