// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace WAYWF.UI.Win32
{
	[Flags]
	enum SHGFI
	{
		SHGFI_SMALLICON = 0x000000001,
		SHGFI_ICON = 0x000000100,
		SHGFI_USEFILEATTRIBUTES = 0x000000010,
	}
}
