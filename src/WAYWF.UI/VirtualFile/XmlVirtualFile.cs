// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;
using System.Text;

namespace WAYWF.UI.VirtualFile
{
	sealed class XmlVirtualFile : VirtualFileBase
	{
		public XmlVirtualFile(string baseFileName, string xmlContent)
			: base(baseFileName + ".xml")
		{
			_xmlContent = xmlContent;
		}

		public override string Extension => ".xml";
		public override byte[] GenerateContent() => Encoding.UTF8.GetBytes(_xmlContent);

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly string _xmlContent;
	}
}
