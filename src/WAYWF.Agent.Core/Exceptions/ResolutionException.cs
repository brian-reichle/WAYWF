// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.Serialization;
using WAYWF.Agent.Core.CorDebugApi;
using WAYWF.Agent.Data;

namespace WAYWF.Agent.Core
{
	[Serializable]
	sealed class ResolutionException : Exception
	{
		public ResolutionException(MetaDataToken token)
			: base("Unable to resolve the " + token.TokenType + " token " + token)
		{
		}

		public ResolutionException(CorElementType type)
			: base("Unable to resolve " + type)
		{
		}

		public ResolutionException()
			: base("Unable to resolve token.")
		{
		}

		public ResolutionException(string message)
			: base(message)
		{
		}

		ResolutionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public ResolutionException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
