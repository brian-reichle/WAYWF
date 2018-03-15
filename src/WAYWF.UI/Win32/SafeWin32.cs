// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace WAYWF.UI.Win32
{
	static class SafeWin32
	{
		public static int? GetProcessIDAtPoint(int x, int y)
		{
			var p = new POINT();
			p.X = x;
			p.Y = y;

			var hWnd = NativeMethods.WindowFromPoint(p);

			if (hWnd == IntPtr.Zero)
			{
				return null;
			}

			var pid = GetOwningPid(hWnd);

			return pid;
		}

		public static SafeHGlobal GlobalAlloc(int size)
		{
			const int GMEM_MOVEABLE = 0x0002;
			const int GMEM_ZEROINIT = 0x0040;

			var hMem = NativeMethods.GlobalAlloc(GMEM_MOVEABLE | GMEM_ZEROINIT, (IntPtr)size);

			if (hMem == null || hMem.IsInvalid)
			{
				throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
			}

			return hMem;
		}

		[SuppressMessage("Microsoft.Usage", "CA2219:DoNotRaiseExceptionsInExceptionClauses")]
		public static BitmapSource GetIcon(string extension)
		{
			var info = new SHFILEINFO();
			BitmapSource result;

			RuntimeHelpers.PrepareConstrainedRegions();
			try { }
			finally
			{
				if (NativeMethods.SHGetFileInfo(
					extension,
					FileAttributes.FILE_ATTRIBUTE_NORMAL,
					ref info,
					Marshal.SizeOf(typeof(SHFILEINFO)),
					SHGFI.SHGFI_ICON | SHGFI.SHGFI_SMALLICON | SHGFI.SHGFI_USEFILEATTRIBUTES) == IntPtr.Zero)
				{
					throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
				}

				if (info.hIcon == IntPtr.Zero)
				{
					result = null;
				}
				else
				{
					try
					{
						result = Imaging.CreateBitmapSourceFromHIcon(info.hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
					}
					finally
					{
						NativeMethods.DestroyIcon(info.hIcon);
					}
				}
			}

			return result;
		}

		[SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "WAYWF.UI.Win32.NativeMethods.GetWindowThreadProcessId(System.IntPtr,System.Int32@)")]
		static int GetOwningPid(IntPtr hWnd)
		{
			NativeMethods.GetWindowThreadProcessId(hWnd, out var pid);
			var error = Marshal.GetLastWin32Error();

			if (error != 0)
			{
				throw new Win32Exception(error);
			}

			return pid;
		}
	}
}
