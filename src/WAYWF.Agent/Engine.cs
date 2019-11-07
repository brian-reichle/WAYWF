// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
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

		public void Run(Stream stream, ILog log)
		{
			log?.WriteFormattedLine("Attaching to process {0}.", _options.ProcessID);

			var callback = new ManagedCallback();
			var handle = CorDebuggerHelper.OpenProcess(_options.ProcessID);

			var debugger = CorDebuggerHelper.CreateDebuggingInterfaceForProcess(_options.ProcessID, handle);
			debugger.Initialize();
			debugger.SetManagedHandler(callback);

			var process = debugger.DebugActiveProcess(handle);

			callback.AwaitAttachComplete();

			log?.WriteLine("Collecting initial state data.");

			var builder = new RuntimeProcessBuilder(callback, handle, process);
			builder.ImportFromHandle();
			builder.ImportStandardProcessDetails();

			if (_options.WalkHeap)
			{
				log?.WriteLine("Walking heap.");
				builder.WalkHeap(process);
			}

			var data = builder.GetProcess();

			try
			{
				if (_options.WaitSeconds > 0)
				{
					log?.WriteFormattedLine("Resuming for {0} seconds.", _options.WaitSeconds);

					builder.MarkStartTime();
					process.Continue();
					callback.WaitTimeOrTermination(TimeSpan.FromSeconds(_options.WaitSeconds));
					process.Stop();
				}

				log?.WriteLine("Detaching from target process.");

				callback.FlushSteppers();
				FlushQueuedCallbacks(process);
				process.Detach();
			}
			catch (COMException ex) when (ex.ErrorCode == HResults.CORDBG_E_PROCESS_TERMINATED)
			{
				log?.WriteLine("Target process terminated.");
			}

			debugger.Terminate();

			log?.WriteLine("Writing results.");

			WriteData(stream, data);

			log?.WriteLine("Done.");
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
