// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.Serialization;

namespace WAYWF.Agent
{
	/// <summary>
	/// The shit hit the fan!
	/// </summary>
	[Serializable]
	sealed class ShitFanContactException : Exception
	{
		public ShitFanContactException()
			: base("what should be an impossible situation somehow occured.")
		{
		}

		public ShitFanContactException(string message)
			: base(message)
		{
		}

		ShitFanContactException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public ShitFanContactException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
