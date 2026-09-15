// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Reflection;
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test;

[TestFixture]
class CallingConventionsExtensionsTests
{
	[TestCase(CallingConventions.Standard, ExpectedResult = false)]
	[TestCase(CallingConventions.Standard | CallingConventions.HasThis, ExpectedResult = true)]
	[TestCase(CallingConventions.Standard | CallingConventions.ExplicitThis | CallingConventions.HasThis, ExpectedResult = false)]
	[TestCase(CallingConventions.VarArgs, ExpectedResult = false)]
	[TestCase(CallingConventions.VarArgs | CallingConventions.HasThis, ExpectedResult = true)]
	[TestCase(CallingConventions.VarArgs | CallingConventions.ExplicitThis | CallingConventions.HasThis, ExpectedResult = false)]
	public bool HasImplicitThis(CallingConventions value)
		=> value.HasImplicitThis();
}
