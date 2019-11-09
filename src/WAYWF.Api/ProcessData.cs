// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.ComponentModel;
using System.IO;
using WAYWF.Api.Win32;

namespace WAYWF.Api
{
	public sealed class ProcessData : INotifyPropertyChanged
	{
		public static ProcessData FromPID(int pid)
		{
			try
			{
				using var process = SafeWin32.GetProcessById(pid);
				var is32Bit = !Environment.Is64BitOperatingSystem || process.IsWow64Process();
				var imageName = process.QueryFullProcessImageName();
				var processName = Path.GetFileNameWithoutExtension(imageName);
				return new ProcessData(pid, processName, is32Bit);
			}
			catch (Win32Exception ex) when (IsErrorCodeIgnoreable(ex))
			{
				return null;
			}
			catch (ArgumentException)
			{
				return null;
			}
		}

		ProcessData(int processID, string processName, bool is32Bit)
		{
			ProcessID = processID;
			ProcessName = processName;
			Is32Bit = is32Bit;
		}

		public int ProcessID { get; }
		public string ProcessName { get; }
		public bool Is32Bit { get; }

		static bool IsErrorCodeIgnoreable(Win32Exception ex)
		{
			switch (ex.NativeErrorCode)
			{
				case Win32ErrorCodes.ERROR_ACCESS_DENIED:
				case Win32ErrorCodes.ERROR_GEN_FAILURE:
				case Win32ErrorCodes.ERROR_INVALID_PARAMETER:
					return true;
			}

			return false;
		}

		#region INotifyPropertyChanged Members

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add { }
			remove { }
		}

		#endregion
	}
}
