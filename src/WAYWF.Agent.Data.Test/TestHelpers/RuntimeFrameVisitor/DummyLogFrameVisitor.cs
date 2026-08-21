// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;

namespace WAYWF.Agent.Data.Test;

sealed class DummyLogFrameVisitor : DummyBaseFrameVisitor
{
	public List<Record> Records { get; } = [];

	protected override void Visit(RuntimeFrame frame, Type identifiedType)
	{
		Records.Add(new Record(frame, identifiedType));
	}

	public sealed class Record
	{
		public Record(RuntimeFrame frame, Type identifiedType)
		{
			Frame = frame;
			IdentifiedType = identifiedType;
		}

		public RuntimeFrame Frame { get; }
		public Type IdentifiedType { get; }
	}
}
