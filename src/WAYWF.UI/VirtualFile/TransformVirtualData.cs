// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.IO;
using System.Reflection;

namespace WAYWF.UI.VirtualFile
{
	sealed class TransformVirtualData : VirtualFileBase
	{
		public static readonly TransformVirtualData Instance = new TransformVirtualData();

		TransformVirtualData()
			: base(HtmlTranslator.TransformFilename)
		{
		}

		public override string Extension => ".xslt";

		public override byte[] GenerateContent()
		{
			var directory = AppDomain.CurrentDomain.BaseDirectory;
			var filename = Path.Combine(directory, HtmlTranslator.TransformFilename);

			try
			{
				using var stream = File.Open(filename, FileMode.Open);
				return GetBytes(stream);
			}
			catch (FileNotFoundException)
			{
			}

			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WAYWF.UI.Resources.waywf.xslt"))
			{
				return GetBytes(stream);
			}
		}

		static byte[] GetBytes(Stream stream)
		{
			var result = new byte[stream.Length];
			stream.Read(result, 0, result.Length);
			return result;
		}
	}
}
