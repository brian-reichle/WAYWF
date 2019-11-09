// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using WAYWF.Agent.Core.Win32;
using WAYWF.Agent.Data;

namespace WAYWF.Agent.Core
{
	static class RuntimeWindowLoader
	{
		public static unsafe RuntimeWindow[] Load(int pid)
		{
			var host = new Host(pid);
			var handle = new GCHandle();

			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				handle = GCHandle.Alloc(host);

				if (!NativeMethods.EnumWindows(Callback, (IntPtr)handle))
				{
					throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
				}
			}
			finally
			{
				if (handle.IsAllocated)
				{
					handle.Free();
				}
			}

			return host._windows.ToArray();
		}

		static bool Callback(IntPtr hwnd, IntPtr lParam)
		{
			var host = (Host)GCHandle.FromIntPtr(lParam).Target;
			Add(host, hwnd);
			return true;
		}

		static void Add(Host host, IntPtr hwnd)
		{
			var threadID = NativeMethods.GetWindowThreadProcessId(hwnd, out var processID);

			if (processID == host._pid)
			{
				var isVisible = NativeMethods.IsWindowVisible(hwnd);
				var isEnabled = NativeMethods.IsWindowEnabled(hwnd);

				host._windows.Add(
					new RuntimeWindow(
						threadID,
						hwnd,
						GetOwner(hwnd),
						GetWindowText(host._builder, hwnd),
						GetWindowClassName(host._builder, hwnd),
						isVisible,
						isEnabled));
			}
		}

		static IntPtr GetOwner(IntPtr hwnd)
		{
			// Should throw an exception if GetWindow fails with an error code, but it seems to think
			// that the argument is invalid if it simply doesnt have an owner.
			return NativeMethods.GetWindow(hwnd, NativeMethods.GW_OWNER);
		}

		static string GetWindowText(StringBuilder builder, IntPtr hwnd)
		{
			var size = NativeMethods.GetWindowTextLength(hwnd);

			if (size == 0)
			{
				return null;
			}

			size++;
			builder.EnsureCapacity(size);
			var length = NativeMethods.GetWindowText(hwnd, builder, size);

			if (length == 0)
			{
				var hr = Marshal.GetHRForLastWin32Error();

				if (hr < 0)
				{
					throw Marshal.GetExceptionForHR(hr);
				}

				return null;
			}

			builder.Length = length;
			return builder.ToString();
		}

		static string GetWindowClassName(StringBuilder builder, IntPtr hwnd)
		{
			builder.EnsureCapacity(256);
			var length = NativeMethods.GetClassName(hwnd, builder, builder.Capacity);

			if (length == 0)
			{
				var hr = Marshal.GetHRForLastWin32Error();

				if (hr < 0)
				{
					throw Marshal.GetExceptionForHR(hr);
				}

				return null;
			}

			builder.Length = length;
			return builder.ToString();
		}

		sealed class Host
		{
			public Host(int pid)
			{
				_pid = pid;
				_windows = new List<RuntimeWindow>();
				_builder = new StringBuilder();
			}

			public readonly int _pid;
			public readonly List<RuntimeWindow> _windows;
			public readonly StringBuilder _builder;
		}
	}
}
