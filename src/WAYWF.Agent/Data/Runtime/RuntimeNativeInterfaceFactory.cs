// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using WAYWF.Agent.CorDebugApi;
using WAYWF.Agent.Win32;

namespace WAYWF.Agent.Data
{
	sealed class RuntimeNativeInterfaceFactory
	{
		public RuntimeNativeInterfaceFactory(ProcessHandle handle, ICorDebugProcess process)
		{
			_handle = handle;
			_process = process;
		}

		public RuntimeNativeInterface[] GetInterfaces(CORDB_ADDRESS[] interfacePointers)
		{
			var result = new RuntimeNativeInterface[interfacePointers.Length];

			for (var i = 0; i < result.Length; i++)
			{
				result[i] = GetInterface(interfacePointers[i]);
			}

			return result;
		}

		public RuntimeNativeInterface GetInterface(CORDB_ADDRESS interfacePointer)
		{
			var vtbl = _process.ReadAddress(interfacePointer);
			return new RuntimeNativeInterface(Map(interfacePointer), Map(vtbl));
		}

		public RuntimeVirtualAddress Map(CORDB_ADDRESS address)
		{
			var ptr = (IntPtr)address;
			var size = (IntPtr)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION));
			var count = NativeMethods.VirtualQueryEx(_handle, ptr, out var info, size);

			if (count != size || info.AllocationProtect == 0 || (info.Type & MEM_FLAGS.MEM_IMAGE) == 0)
			{
				return new RuntimeVirtualAddress(address);
			}

			var moduleName = _handle.GetModuleBaseName(info.AllocationBase);

			if (string.IsNullOrEmpty(moduleName))
			{
				return new RuntimeVirtualAddress(address);
			}

			var offset = (int)((long)ptr - (long)info.AllocationBase);
			return new RuntimeVirtualAddress(address, moduleName, offset);
		}

		readonly ProcessHandle _handle;
		readonly ICorDebugProcess _process;
	}
}
