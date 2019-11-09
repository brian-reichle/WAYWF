// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.Serialization;

namespace WAYWF.Agent.Core
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
			return errorCode switch
			{
				ErrorCodes.AlreadyAttached => "A debugger is already attached to the specified process.",
				ErrorCodes.UnsupportedCLR => "Unsupported CLR Version.",
				ErrorCodes.NoCLRLoaded => "CLR Not Loaded.",
				_ => "ErrorCodes: " + errorCode,
			};
		}
	}
}
