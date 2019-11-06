// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace WAYWF.Agent.Data
{
	sealed class RuntimeVirtualAddress
	{
		public RuntimeVirtualAddress(MemoryAddress address)
			: this(address, null, 0)
		{
		}

		public RuntimeVirtualAddress(MemoryAddress address, string image, int offset)
		{
			Address = address;
			Image = image;
			Offset = offset;
		}

		public MemoryAddress Address { get; }
		public string Image { get; }
		public int Offset { get; }
		public override string ToString() => Image == null ? Address.ToString() : (Image + "+" + Offset);
	}
}
