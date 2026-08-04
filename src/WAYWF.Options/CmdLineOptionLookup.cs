// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace WAYWF.Options
{
	sealed class CmdLineOptionLookup : IEnumerable<CmdLineOption>
	{
		public CmdLineOptionLookup()
		{
			_lookup = new Dictionary<string, CmdLineOption>();
		}

		public void Add(string name, bool takesArg, Action<CmdLineOptions, string> action)
		{
			_lookup.Add(name, new CmdLineOption(name, takesArg, action));
		}

		public bool TryGetValue(string name, out CmdLineOption option)
		{
			return _lookup.TryGetValue(name, out option);
		}

		public IEnumerator<CmdLineOption> GetEnumerator() => _lookup.Values.GetEnumerator();

		[DebuggerStepThrough]
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		readonly Dictionary<string, CmdLineOption> _lookup;
	}
}
