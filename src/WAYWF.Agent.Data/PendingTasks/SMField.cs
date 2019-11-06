// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;

namespace WAYWF.Agent.Data
{
	[DebuggerDisplay("{Name,nq}")]
	public sealed class SMField
	{
		public SMField(MetaDataToken fieldToken, string name)
		{
			FieldToken = fieldToken;
			Name = name;
		}

		public MetaDataToken FieldToken { get; }
		public string Name { get; }
	}
}
