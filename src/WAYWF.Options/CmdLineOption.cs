// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace WAYWF.Options
{
	sealed class CmdLineOption
	{
		public CmdLineOption(string name, bool takesArg, Action<CmdLineOptions, string> action)
		{
			Name = name;
			TakesArgs = takesArg;
			Action = action;
		}

		public string Name { get; }
		public bool TakesArgs { get; }
		public Action<CmdLineOptions, string> Action { get; }
	}
}
