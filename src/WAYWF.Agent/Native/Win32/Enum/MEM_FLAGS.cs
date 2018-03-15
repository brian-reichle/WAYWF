// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace WAYWF.Agent.Win32
{
	[Flags]
	enum MEM_FLAGS
	{
		/// <summary>
		/// Indicates that the memory pages within the region are mapped into the view of an image section.
		/// </summary>
		MEM_IMAGE = 0x1000000,

		/// <summary>
		/// Indicates that the memory pages within the region are mapped into the view of a section.
		/// </summary>
		MEM_MAPPED = 0x40000,

		/// <summary>
		/// Indicates that the memory pages within the region are private (that is, not shared by other processes).
		/// </summary>
		MEM_PRIVATE = 0x20000,
	}
}
