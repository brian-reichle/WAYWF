// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.IO;
using System.Reflection;

namespace WAYWF.Options
{
	static class OptionReport
	{
		public static void WriteCaptions(TextWriter text)
		{
			var assembly = Assembly.GetExecutingAssembly();
			var version = assembly.GetName().Version;
			var product = GetAssemblyAttribute<AssemblyProductAttribute>(assembly).Product;
			var copyright = GetAssemblyAttribute<AssemblyCopyrightAttribute>(assembly).Copyright;

			text.WriteLine(product + " (" + version + ")");
			text.WriteLine(copyright);
			text.WriteLine();
		}

		public static void WriteUsage(TextWriter text)
		{
#pragma warning disable SA1005 // Single line comments must begin with single space
			//              0         1         2         3         4         5         6         7         8
			//              +----+----+----+----+----+----+----+----+----+----+----+----+----+----+----+----+
#pragma warning restore SA1005 // Single line comments must begin with single space
			text.WriteLine("Usage:");
			text.WriteLine("    waywf --pid <processId> --output process.xml");
			text.WriteLine("    waywf --pid <processId> --output process.xml --wait 5");
			text.WriteLine();
			text.WriteLine("Options:");
			text.WriteLine("    --pid <processId>     Specifies which process to attach to.");
			text.WriteLine("    --output <filename>   Write the xml to the specified file. If this option");
			text.WriteLine("                          is not specified then the xml is written to stdout.");
			text.WriteLine("    --wait <seconds>      After attaching, WAYWF will wait the specified number");
			text.WriteLine("                          of seconds to see which frames finish.");
			text.WriteLine("    --walkheap            Walk the managed object heap in an attempt to gather");
			text.WriteLine("                          more information about what is happening.");
			text.WriteLine("    --verbose             Write progress information to STDERR.");
			text.WriteLine();
			text.WriteLine("Only --pid is required, all other options are optional.");
			text.WriteLine();
		}

		static T GetAssemblyAttribute<T>(Assembly assembly)
			where T : Attribute
		{
			return (T)Attribute.GetCustomAttribute(assembly, typeof(T));
		}
	}
}
