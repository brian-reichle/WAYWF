// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;

namespace WAYWF.Agent.Data
{
	[DebuggerDisplay("Internal Frame: {InternalFrameType}")]
	public sealed class RuntimeInternalFrame : RuntimeFrame
	{
		public RuntimeInternalFrame(RuntimeInternalFrameKind internalFrameType)
		{
			InternalFrameType = internalFrameType;
		}

		public RuntimeInternalFrameKind InternalFrameType { get; }

		public override void Apply(IRuntimeFrameVisitor visitor) => visitor.Visit(this);
	}
}
