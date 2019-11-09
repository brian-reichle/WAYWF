// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;

namespace WAYWF.UI
{
	static class HtmlTranslator
	{
		public const string TransformFilename = "waywf.xslt";

		public static XslCompiledTransform Instance => _instance ?? (_instance = GetTransform());

		public static void Transform(string xmlContent, Stream writeStream)
		{
			using var writer = XmlWriter.Create(writeStream);
			Transform(xmlContent, writer);
			writer.Flush();
		}

		public static void Transform(string xmlContent, XmlWriter writer)
		{
			using var readStream = new StringReader(xmlContent);
			using var reader = CreateReader(readStream);
			Instance.Transform(reader, writer);
		}

		static XslCompiledTransform GetTransform()
		{
			var directory = AppDomain.CurrentDomain.BaseDirectory;
			var filename = Path.Combine(directory, TransformFilename);

			try
			{
				using var stream = File.Open(filename, FileMode.Open);
				return FromStream(stream);
			}
			catch (FileNotFoundException)
			{
			}

			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WAYWF.UI.Resources.waywf.xslt"))
			{
				return FromStream(stream);
			}
		}

		static XslCompiledTransform FromStream(Stream stream)
		{
			var result = new XslCompiledTransform();
			var transformSettings = new XsltSettings(false, false);

			using (var reader = CreateReader(stream))
			{
				result.Load(reader, transformSettings, NullResolver.Instance);
			}

			return result;
		}

		static XmlReader CreateReader(Stream stream) => CreateReader(new StreamReader(stream));

		static XmlReader CreateReader(TextReader reader)
		{
			var readerSettings = new XmlReaderSettings()
			{
				ConformanceLevel = ConformanceLevel.Document,
				DtdProcessing = DtdProcessing.Prohibit,
				XmlResolver = null,
				ValidationType = ValidationType.None,
				ValidationFlags = System.Xml.Schema.XmlSchemaValidationFlags.None,
			};

			return XmlReader.Create(reader, readerSettings);
		}

		sealed class NullResolver : XmlResolver
		{
			public static readonly NullResolver Instance = new NullResolver();

			NullResolver()
			{
			}

			public override ICredentials Credentials
			{
				set { }
			}

			public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn) => null;
		}

		static XslCompiledTransform _instance;
	}
}
