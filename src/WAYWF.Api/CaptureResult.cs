// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace WAYWF.Api
{
	public sealed class CaptureResult
	{
		public CaptureResult(string stdOut, int exitCode)
		{
			StandardOutput = stdOut;
			ExitCode = exitCode;
		}

		public string StandardOutput { get; }
		public int ExitCode { get; }
	}
}
