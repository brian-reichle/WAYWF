// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using WAYWF.Agent.Core.CorDebugApi;

namespace WAYWF.Agent.Core
{
	static class ProcessExtensions
	{
		public static Version GetVersion(this ICorDebugProcess process)
		{
			if (process is ICorDebugProcess2 process2)
			{
				process2.GetVersion(out var version);
				return new Version(version.dwMajor, version.dwMinor, version.dwBuild, version.dwSubBuild);
			}

			return null;
		}

		public static unsafe int ReadInt(this ICorDebugProcess process, CORDB_ADDRESS address)
		{
			int result;
			process.ReadMemory(address, sizeof(int), (IntPtr)(&result));
			return result;
		}

		public static unsafe CORDB_ADDRESS ReadAddress(this ICorDebugProcess process, CORDB_ADDRESS address)
		{
			var result = default(CORDB_ADDRESS);
			process.ReadMemory(address, IntPtr.Size, (IntPtr)(&result));
			return result;
		}

		public static unsafe byte[] ReadBytes(this ICorDebugProcess process, CORDB_ADDRESS address, int length)
		{
			var result = new byte[length];

			fixed (byte* resultPtr = result)
			{
				process.ReadMemory(address, length, (IntPtr)resultPtr);
			}

			return result;
		}

		public static bool AreGCStructuresValid(this ICorDebugProcess5 process)
		{
			process.GetGCHeapInformation(out var heap);
			return heap.areGCStructuresValid;
		}
	}
}
