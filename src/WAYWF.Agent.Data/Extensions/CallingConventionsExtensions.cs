// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Reflection;

namespace WAYWF.Agent.Data;

public static class CallingConventionsExtensions
{
	public static bool HasImplicitThis(this CallingConventions callingConvention)
	{
		const CallingConventions mask =
			CallingConventions.HasThis |
			CallingConventions.ExplicitThis;

		return (callingConvention & mask) == CallingConventions.HasThis;
	}
}
