// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WAYWF.Api
{
	public static class Launcher
	{
		public static Task<CaptureResult> CaptureAsync(ProcessData data, CaptureConfig config, Action<string> errorOutCallback)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));

			return CaptureAsync(data.ProcessID, data.Is32Bit, config, errorOutCallback);
		}

		public static async Task<CaptureResult> CaptureAsync(int processId, bool is32bit, CaptureConfig config, Action<string> errorOutCallback)
		{
			if (config == null) throw new ArgumentNullException(nameof(config));

			var info = CreateStartInfo(processId, is32bit, errorOutCallback != null, config);

			using var process = new Process();
			process.StartInfo = info;

			if (errorOutCallback != null)
			{
				process.ErrorDataReceived += delegate (object sender, DataReceivedEventArgs e)
				{
					errorOutCallback(e.Data);
				};
			}

			try
			{
				process.Start();
			}
			catch (Win32Exception ex) when (ex.NativeErrorCode == Win32.Win32ErrorCodes.ERROR_FILE_NOT_FOUND)
			{
				errorOutCallback?.Invoke("Error starting '" + info.FileName + "', file not found.");
				return new CaptureResult(string.Empty, int.MinValue);
			}

			if (errorOutCallback != null)
			{
				process.BeginErrorReadLine();
			}

			var stdOutTaskTask = process.StandardOutput.ReadToEndAsync();
			var resultTask = process.WaitForExitAsync();
			await Task.WhenAll(stdOutTaskTask, resultTask).ConfigureAwait(false);
			return new CaptureResult(stdOutTaskTask.Result, resultTask.Result);
		}

		static ProcessStartInfo CreateStartInfo(int processId, bool is32bit, bool redirectError, CaptureConfig config)
		{
			var filename = is32bit ? "WAYWF.Agent.x86.exe" : "WAYWF.Agent.exe";
			var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			var path = Path.Combine(baseDirectory, filename);

			var info = new ProcessStartInfo()
			{
				RedirectStandardOutput = true,
				UseShellExecute = false,
				WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
				FileName = path,
				CreateNoWindow = true,
				RedirectStandardError = redirectError,
			};

			var builder = new StringBuilder();

			builder.Append(" --pid ");
			builder.Append(processId);

			if (config.Verbose)
			{
				builder.Append(" --verbose");
			}

			if (config.WalkHeap)
			{
				builder.Append(" --walkheap");
			}

			if (config.WaitSeconds > 0)
			{
				builder.Append(" --wait ");
				builder.Append(config.WaitSeconds);
			}

			info.Arguments = builder.ToString();
			return info;
		}

		static Task<int> WaitForExitAsync(this Process process)
		{
			var source = new TaskCompletionSource<int>();

			if (process.HasExited)
			{
				source.SetResult(process.ExitCode);
			}
			else
			{
				process.Exited += delegate
				{
					source.TrySetResult(process.ExitCode);
				};

				process.EnableRaisingEvents = true;
			}

			return source.Task;
		}
	}
}
