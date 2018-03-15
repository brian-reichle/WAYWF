// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace WAYWF.Agent
{
	static class ErrorCodes
	{
		public const int UnknownError = -1;
		public const int Success = 0;

		public const int InvalidArguments = 1;
		public const int IOError = 2;
		public const int ProcessAccessDenied = 3;
		public const int OutputAccessDenied = 4;

		public const int NoProcess = 11;
		public const int BitnessMismatch = 12;
		public const int AlreadyAttached = 13;
		public const int UnsupportedCLR = 14;
		public const int NoCLRLoaded = 15;
	}
}
