// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;

namespace WAYWF.Agent.Data.Test
{
	sealed class DummyLogValueVisitor : DummyBaseValueVisitor
	{
		public List<Record> Records { get; } = [];

		protected override void Visit(RuntimeValue value, Type identifiedType)
		{
			Records.Add(new Record(value, identifiedType));
		}

		public sealed class Record
		{
			public Record(RuntimeValue value, Type identifiedType)
			{
				Value = value;
				IdentifiedType = identifiedType;
			}

			public RuntimeValue Value { get; }
			public Type IdentifiedType { get; }
		}
	}
}
