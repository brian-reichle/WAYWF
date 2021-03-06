// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.Serialization;

namespace WAYWF.Agent.Core.IL
{
	[Serializable]
	public class ILException : Exception
	{
		public ILException()
		{
		}

		public ILException(string message)
			: base(message)
		{
		}

		protected ILException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public ILException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
