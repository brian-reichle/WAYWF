// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace WAYWF.Agent.Win32
{
	[Flags]
	enum AccessOptions
	{
		PROCESS_VM_READ = 0x0010,
		PROCESS_DUP_HANDLE = 0x0040,
		PROCESS_QUERY_INFORMATION = 0x0400,
		SYNCHRONIZE = 0x100000,
	}
}
