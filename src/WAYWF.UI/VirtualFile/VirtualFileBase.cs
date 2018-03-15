// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace WAYWF.UI.VirtualFile
{
	abstract class VirtualFileBase
	{
		protected VirtualFileBase(string filename)
		{
			FileName = filename;
		}

		public abstract string Extension { get; }
		public abstract byte[] GenerateContent();

		public string FileName { get; }
	}
}
