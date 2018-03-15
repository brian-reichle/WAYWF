// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml;
using WAYWF.Agent.CorDebugApi;
using WAYWF.Agent.Data;
using WAYWF.Options;

namespace WAYWF.Agent
{
	sealed class Engine
	{
		public Engine(CmdLineOptions options)
		{
			_options = options;
		}

		public void Run()
		{
			var sw = new Stopwatch();
			var stream = OpenStream();

			if (_options.Verbose)
			{
				Console.Error.WriteLine("0.000: Attaching to process {0}.", _options.ProcessID);
				sw.Start();
			}

			var callback = new ManagedCallback();
			var handle = CorDebuggerHelper.OpenProcess(_options.ProcessID);

			var debugger = CorDebuggerHelper.CreateDebuggingInterfaceForProcess(_options.ProcessID, handle);
			debugger.Initialize();
			debugger.SetManagedHandler(callback);

			var process = debugger.DebugActiveProcess(handle);

			callback.AwaitAttachComplete();

			if (_options.Verbose)
			{
				Console.Error.WriteLine(string.Format("{0:0.000}: Collecting initial state data.", sw.Elapsed.TotalSeconds));
			}

			var builder = new RuntimeProcessBuilder(callback, handle, process);
			builder.ImportFromHandle();
			builder.ImportStandardProcessDetails();

			if (_options.WalkHeap)
			{
				if (_options.Verbose)
				{
					Console.Error.WriteLine(string.Format("{0:0.000}: Walking heap.", sw.Elapsed.TotalSeconds));
				}

				builder.WalkHeap(process);
			}

			var data = builder.GetProcess();

			try
			{
				if (_options.WaitSeconds > 0)
				{
					if (_options.Verbose)
					{
						Console.Error.WriteLine(string.Format("{0:0.000}: Resuming for {1} seconds.", sw.Elapsed.TotalSeconds, _options.WaitSeconds));
					}

					builder.MarkStartTime();
					process.Continue();
					callback.WaitTimeOrTermination(TimeSpan.FromSeconds(_options.WaitSeconds));
					process.Stop();
				}

				if (_options.Verbose)
				{
					Console.Error.WriteLine(string.Format("{0:0.000}: Detaching from target process.", sw.Elapsed.TotalSeconds));
				}

				callback.FlushSteppers();
				FlushQueuedCallbacks(process);
				process.Detach();
			}
			catch (COMException ex) when (ex.ErrorCode == HResults.CORDBG_E_PROCESS_TERMINATED)
			{
				if (_options.Verbose)
				{
					Console.Error.WriteLine(string.Format("{0:0.000}: Target process terminated.", sw.Elapsed.TotalSeconds));
				}
			}

			debugger.Terminate();

			if (_options.Verbose)
			{
				Console.Error.WriteLine(string.Format("{0:0.000}: Writing results.", sw.Elapsed.TotalSeconds));
			}

			WriteData(stream, data);

			if (_options.Verbose)
			{
				Console.Error.WriteLine(string.Format("{0:0.000}: Done.", sw.Elapsed.TotalSeconds));
			}
		}

		void WriteData(Stream stream, RuntimeProcess process)
		{
			var settings = new XmlWriterSettings()
			{
				Indent = true,
				IndentChars = "  ",
			};

			using (var writer = XmlWriter.Create(stream, settings))
			{
				StateFormatter.Format(writer, process, _options);
			}
		}

		Stream OpenStream()
		{
			if (_options.OutputFileName == null)
			{
				return Console.OpenStandardOutput();
			}
			else
			{
				try
				{
					var stream = new FileStream(_options.OutputFileName, FileMode.Create, FileAccess.Write);
					stream.SetLength(0);
					return stream;
				}
				catch (UnauthorizedAccessException ex)
				{
					throw new CodedErrorException(ErrorCodes.OutputAccessDenied, "Could not open output file for writing.", ex);
				}
			}
		}

		static void FlushQueuedCallbacks(ICorDebugProcess process)
		{
			while (process.HasQueuedCallbacks())
			{
				process.Continue();
				Thread.Sleep(0);
				process.Stop();
			}
		}

		readonly CmdLineOptions _options;
	}
}
