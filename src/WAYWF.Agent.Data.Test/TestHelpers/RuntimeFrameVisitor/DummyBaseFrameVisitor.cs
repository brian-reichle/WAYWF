// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace WAYWF.Agent.Data.Test
{
	abstract class DummyBaseFrameVisitor : IRuntimeFrameVisitor
	{
		public virtual void Visit(RuntimeILFrame frame) => Visit(frame, typeof(RuntimeILFrame));
		public virtual void Visit(RuntimeInternalFrame frame) => Visit(frame, typeof(RuntimeInternalFrame));

		protected virtual void Visit(RuntimeFrame frame, Type identifiedType) => throw new InvalidOperationException($"Did not expect the {identifiedType.Name} overload to be called.");
	}
}
