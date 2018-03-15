// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.Serialization;

namespace WAYWF.Options
{
	[Serializable]
	sealed class OptionException : Exception
	{
		public OptionException()
		{
		}

		public OptionException(string message)
			: base(message)
		{
		}

		OptionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public OptionException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
