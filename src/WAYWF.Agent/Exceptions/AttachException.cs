// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.Serialization;

namespace WAYWF.Agent
{
	[Serializable]
	sealed class AttachException : CodedErrorException
	{
		public AttachException(int errorCode)
			: base(errorCode, GetMessage(errorCode))
		{
		}

		public AttachException(int errorCode, string message)
			: base(errorCode, message)
		{
		}

		public AttachException(int errorCode, string message, Exception innerException)
			: base(errorCode, message, innerException)
		{
		}

		AttachException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public static AttachException ProcessTerminatedBeforeAttaching(int pid)
		{
			return new AttachException(
				ErrorCodes.NoProcess,
				"Process " + pid + " terminated before attaching.");
		}

		static string GetMessage(int errorCode)
		{
			switch (errorCode)
			{
				case ErrorCodes.AlreadyAttached: return "A debugger is already attached to the specified process.";
				case ErrorCodes.UnsupportedCLR: return "Unsupported CLR Version.";
				case ErrorCodes.NoCLRLoaded: return "CLR Not Loaded.";
				default: return "ErrorCodes: " + errorCode;
			}
		}
	}
}
