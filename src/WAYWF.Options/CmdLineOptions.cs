// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace WAYWF.Options
{
	sealed class CmdLineOptions
	{
		public int ProcessID { get; private set; }
		public string OutputFileName { get; private set; }
		public int WaitSeconds { get; private set; }
		public bool WalkHeap { get; private set; }
		public bool Verbose { get; private set; }

		bool _waitSpecified;

		public void Parse(string[] args)
		{
			for (var i = 0; i < args.Length; i++)
			{
				var arg = args[i];

				if (!arg.StartsWith("--", StringComparison.Ordinal))
				{
					throw new OptionException("Unknown option \"" + arg + "\".");
				}

				var index = arg.IndexOf('=');
				string optionName;

				if (index < 0)
				{
					optionName = arg.Substring(2);
					arg = null;
				}
				else
				{
					optionName = arg.Substring(2, index - 2);
					arg = arg.Substring(index + 1);
				}

				if (!_lookup.TryGetValue(optionName, out var option))
				{
					throw new OptionException("Unknown option \"--" + optionName + "\".");
				}

				if (!option.TakesArgs)
				{
					if (arg != null)
					{
						throw new OptionException("Option \"--" + optionName + "\" does not take an argument.");
					}
				}
				else if (arg == null)
				{
					i++;

					if (i >= args.Length)
					{
						throw new OptionException("Option \"--" + optionName + "\" requires an argument.");
					}

					arg = args[i];
				}

				option.Action(this, arg);
			}

			ValidateOptions();
		}

		static void Set_PID(CmdLineOptions options, string arg)
		{
			if (!int.TryParse(arg, out var pid) || pid == 0)
			{
				throw new OptionException("Invalid process id.");
			}
			else if (options.ProcessID != 0)
			{
				throw new OptionException("Multiple target processes specified.");
			}

			options.ProcessID = pid;
		}

		static void Set_Output(CmdLineOptions options, string arg)
		{
			if (options.OutputFileName != null)
			{
				throw new OptionException("'--output' specified multiple times.");
			}

			options.OutputFileName = arg;
		}

		static void SetWaitSeconds(CmdLineOptions options, string arg)
		{
			int seconds;

			if (options._waitSpecified)
			{
				throw new OptionException("--wait specified mulitiple times.");
			}
			else if (!int.TryParse(arg, out seconds))
			{
				throw new OptionException("Invalid wait duration.");
			}

			options._waitSpecified = true;
			options.WaitSeconds = seconds;
		}

		static void SetWalkHeap(CmdLineOptions options, string arg)
		{
			if (options.WalkHeap)
			{
				throw new OptionException("--task specified multiple times.");
			}

			options.WalkHeap = true;
		}

		static void SetVerbose(CmdLineOptions options, string arg)
		{
			if (options.Verbose)
			{
				throw new OptionException("--verbose specified multiple times.");
			}

			options.Verbose = true;
		}

		void ValidateOptions()
		{
			if (ProcessID == 0)
			{
				throw new OptionException("No target process specified.");
			}
		}

		static readonly CmdLineOptionLookup _lookup = new CmdLineOptionLookup()
		{
			{ "pid",      true, Set_PID },
			{ "output",   true, Set_Output },
			{ "wait",     true, SetWaitSeconds },
			{ "walkheap", false, SetWalkHeap },
			{ "verbose",  false, SetVerbose },
		};
	}
}
