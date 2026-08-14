// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Generic;
using System.Reflection;

namespace WAYWF.Agent.Data.Test
{
	sealed class DummyLogMetaTypeVisitor : DummyBaseMetaTypeVisitor
	{
		public List<Record> Records { get; } = [];

		protected override void Visit(MethodBase method, MetaTypeBase type)
		{
			Records.Add(new Record(method, type));
		}

		public sealed class Record
		{
			public Record(MethodBase method, MetaTypeBase type)
			{
				Method = method;
				Type = type;
			}

			public MethodBase Method { get; }
			public MetaTypeBase Type { get; }
		}
	}
}
