// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace WAYWF.Agent.Data
{
	public sealed class RuntimeWindow
	{
		public RuntimeWindow(int threadID, IntPtr handle, IntPtr owner, string caption, string className, bool isVisible, bool isEnabled)
		{
			ThreadID = threadID;
			Handle = handle;
			Owner = owner;
			Caption = caption;
			IsVisible = isVisible;
			IsEnabled = isEnabled;
			ClassName = className;
		}

		public int ThreadID { get; }
		public IntPtr Handle { get; }
		public IntPtr Owner { get; }
		public string Caption { get; }
		public bool IsVisible { get; }
		public bool IsEnabled { get; }
		public string ClassName { get; }
	}
}
