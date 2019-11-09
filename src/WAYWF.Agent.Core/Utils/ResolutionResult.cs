// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace WAYWF.Agent.Core
{
	enum ResolutionResult
	{
		/// <summary>
		/// Matching type successfully located.
		/// </summary>
		Success,

		/// <summary>
		/// Type definately doesn't exist.
		/// </summary>
		Fail,

		/// <summary>
		/// The type could not be found but may yet exist.
		/// </summary>
		NotFound,
	}
}
