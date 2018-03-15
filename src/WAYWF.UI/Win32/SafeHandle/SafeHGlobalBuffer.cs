// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Runtime.InteropServices;

namespace WAYWF.UI.Win32
{
	sealed class SafeHGlobalBuffer : SafeBuffer
	{
		public SafeHGlobalBuffer()
			: base(false)
		{
		}

		protected override bool ReleaseHandle() => true;
	}
}
