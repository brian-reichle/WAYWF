// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Diagnostics;

namespace WAYWF.Agent.Data
{
	[DebuggerDisplay("Assembly: {Name,nq} {Version}")]
	public sealed class MetaAssembly
	{
		public MetaAssembly(string path, string name, Version version, long? publicKeyToken, string locale)
		{
			Path = path;
			Name = name;
			Version = version;
			PublicKeyToken = publicKeyToken;
			Locale = locale;
			IsCorLib = name == "mscorlib";
		}

		public string Path { get; }
		public string Name { get; }
		public Version Version { get; }
		public string Locale { get; }
		public long? PublicKeyToken { get; }
		public bool IsCorLib { get; }
	}
}
