// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace WAYWF.Agent.Data
{
	public sealed class RuntimeNativeInterface
	{
		public RuntimeNativeInterface(RuntimeVirtualAddress interfaceAddress, RuntimeVirtualAddress vtblAddress)
		{
			InterfaceAddress = interfaceAddress;
			VtblAddress = vtblAddress;
		}

		public RuntimeVirtualAddress InterfaceAddress { get; }
		public RuntimeVirtualAddress VtblAddress { get; }
	}
}
