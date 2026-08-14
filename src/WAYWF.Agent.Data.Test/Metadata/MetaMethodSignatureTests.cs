// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;
using System.Reflection;
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test
{
	[TestFixture]
	public class MetaMethodSignatureTests
	{
		[Test]
		public void Constructor_StoresPropertiesVerbatim()
		{
			var resultParam = new MetaVariable(MetaKnownType.Void, null, false, false);
			var param1 = new MetaVariable(MetaKnownType.Int32, "x", false, false);
			var param2 = new MetaVariable(MetaKnownType.String, "y", false, false);
			var parameters = ImmutableArray.Create(param1, param2);

			var signature = new MetaMethodSignature(
				callingConventions: CallingConventions.HasThis | CallingConventions.Standard,
				typeArgs: 1,
				resultParam: resultParam,
				parameters: parameters);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(signature.CallingConventions, Is.EqualTo(CallingConventions.HasThis | CallingConventions.Standard));
				Assert.That(signature.TypeArg, Is.EqualTo(1));
				Assert.That(signature.ResultParam, Is.SameAs(resultParam));
				Assert.That(signature.Parameters, Is.EqualTo(parameters));
			}
		}
	}
}
