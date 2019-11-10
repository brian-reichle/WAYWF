// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;

namespace WAYWF.Agent.Data
{
	public sealed class RuntimeNative
	{
		public RuntimeNative(int processID, string imageName, RuntimeUser user, ImmutableArray<RuntimeWindow> windows)
		{
			ProcessID = processID;
			ImageName = imageName;
			User = user;
			Windows = windows;
		}

		public int ProcessID { get; }
		public string ImageName { get; }
		public RuntimeUser User { get; }
		public ImmutableArray<RuntimeWindow> Windows { get; }
	}
}
