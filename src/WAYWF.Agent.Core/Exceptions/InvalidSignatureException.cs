// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.Serialization;

namespace WAYWF.Agent.Core
{
	[Serializable]
	sealed class InvalidSignatureException : Exception
	{
		public InvalidSignatureException()
			: base("Unexpected end of signature.")
		{
		}

		public InvalidSignatureException(string message)
			: base(message)
		{
		}

		InvalidSignatureException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public InvalidSignatureException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
