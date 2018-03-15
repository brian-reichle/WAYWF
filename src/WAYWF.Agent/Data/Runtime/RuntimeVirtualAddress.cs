// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using WAYWF.Agent.CorDebugApi;

namespace WAYWF.Agent.Data
{
	sealed class RuntimeVirtualAddress
	{
		public RuntimeVirtualAddress(CORDB_ADDRESS address)
			: this(address, null, 0)
		{
		}

		public RuntimeVirtualAddress(CORDB_ADDRESS address, string image, int offset)
		{
			Address = address;
			Image = image;
			Offset = offset;
		}

		public CORDB_ADDRESS Address { get; }
		public string Image { get; }
		public int Offset { get; }
		public override string ToString() => Image == null ? Address.ToString() : (Image + "+" + Offset);
	}
}
