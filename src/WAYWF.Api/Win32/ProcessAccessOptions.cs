// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace WAYWF.Api.Win32
{
	[Flags]
	enum ProcessAccessOptions
	{
		PROCESS_QUERY_LIMITED_INFORMATION = 0x1000,
	}
}
