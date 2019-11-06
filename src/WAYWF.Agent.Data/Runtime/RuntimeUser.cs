// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace WAYWF.Agent.Data
{
	public sealed class RuntimeUser
	{
		public RuntimeUser(string user, string domain)
		{
			User = user;
			Domain = domain;
		}

		public string User { get; }
		public string Domain { get; }
	}
}
