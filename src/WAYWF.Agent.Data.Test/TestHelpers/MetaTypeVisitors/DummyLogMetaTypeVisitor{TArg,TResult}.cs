// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Generic;
using System.Reflection;

namespace WAYWF.Agent.Data.Test;

sealed class DummyLogMetaTypeVisitor<TArg, TResult> : DummyBaseMetaTypeVisitor<TArg, TResult>
{
	public DummyLogMetaTypeVisitor(TResult result)
	{
		Result = result;
	}

	public TResult Result { get; }
	public List<Record> Records { get; } = [];

	protected override TResult Visit(MethodBase method, MetaTypeBase type, TArg arg)
	{
		Records.Add(new Record(method, type, arg));
		return Result;
	}

	public sealed class Record
	{
		public Record(MethodBase method, MetaTypeBase type, TArg argument)
		{
			Method = method;
			Type = type;
			Argument = argument;
		}

		public MethodBase Method { get; }
		public MetaTypeBase Type { get; }
		public TArg Argument { get; }
	}
}
