// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using WAYWF.Options;

namespace WAYWF.Agent
{
	static class Program
	{
		[MTAThread]
		static int Main(string[] args)
		{
			var options = new CmdLineOptions();

			try
			{
				options.Parse(args);
			}
			catch (OptionException ex)
			{
				var text = Console.Error;
				OptionReport.WriteCaptions(text);
				text.WriteLine(ex.Message);
				text.WriteLine();
				OptionReport.WriteUsage(text);
				return ErrorCodes.InvalidArguments;
			}

			if (IsRunningAsAdministrator())
			{
				Process.EnterDebugMode();
			}

			var engine = new Engine(options);

			try
			{
				engine.Run();
			}
			catch (IOException ex)
			{
				OptionReport.WriteCaptions(Console.Error);
				Console.Error.WriteLine(ex.Message);
				return ErrorCodes.IOError;
			}
			catch (CodedErrorException ex)
			{
				OptionReport.WriteCaptions(Console.Error);
				Console.Error.WriteLine(ex.Message);
				return ex.ErrorCode;
			}

			return ErrorCodes.Success;
		}

		static bool IsRunningAsAdministrator()
		{
			using (var identity = WindowsIdentity.GetCurrent())
			{
				return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
			}
		}
	}
}
