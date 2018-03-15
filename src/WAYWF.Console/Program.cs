// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.IO;
using WAYWF.Api;
using WAYWF.Options;

namespace WAYWF.Console
{
	static class Program
	{
		static int Main(string[] args)
		{
			var options = new CmdLineOptions();

			try
			{
				options.Parse(args);
			}
			catch (OptionException ex)
			{
				var text = System.Console.Error;
				OptionReport.WriteCaptions(text);
				text.WriteLine(ex.Message);
				text.WriteLine();
				OptionReport.WriteUsage(text);
				const int InvalidArguments = 1;
				return InvalidArguments;
			}

			var data = ProcessData.FromPID(options.ProcessID);

			if (data == null)
			{
				const int NoProcess = 11;
				return NoProcess;
			}

			var config = new CaptureConfig()
			{
				WaitSeconds = options.WaitSeconds,
				WalkHeap = options.WalkHeap,
			};

			Action<string> reportError = null;

			if (options.Verbose)
			{
				config.Verbose = true;
				reportError = System.Console.WriteLine;
			}

			var result = Launcher.CaptureAsync(data, config, reportError)
				.GetAwaiter().GetResult();

			if (!string.IsNullOrEmpty(options.OutputFileName))
			{
				File.WriteAllText(options.OutputFileName, result.StandardOutput);
			}
			else
			{
				System.Console.Write(result.StandardOutput);
			}

			return result.ExitCode;
		}
	}
}
