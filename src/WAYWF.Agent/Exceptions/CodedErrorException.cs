// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.Serialization;

namespace WAYWF.Agent
{
	[Serializable]
	class CodedErrorException : Exception
	{
		public CodedErrorException(int errorCode, string message)
			: base(message)
		{
			ErrorCode = errorCode;
		}

		public CodedErrorException(int errorCode, string message, Exception innerException)
			: base(message, innerException)
		{
			ErrorCode = errorCode;
		}

		protected CodedErrorException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			ErrorCode = info.GetInt32("ErrorCode");
		}

		public int ErrorCode { get; }

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("ErrorCode", info);
			base.GetObjectData(info, context);
		}
	}
}
