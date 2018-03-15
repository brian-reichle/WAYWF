// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.Serialization;

namespace WAYWF.Agent
{
	[Serializable]
	sealed class InvalidMetaDataException : Exception
	{
		public InvalidMetaDataException()
			: base("Encountered invalid meta-data.")
		{
		}

		public InvalidMetaDataException(string message)
			: base(message)
		{
		}

		InvalidMetaDataException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public InvalidMetaDataException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
