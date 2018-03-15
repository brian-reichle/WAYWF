// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;
using WAYWF.Agent.CorDebugApi;

namespace WAYWF.Agent.Data
{
	[DebuggerDisplay("Internal Frame: {InternalFrameType}")]
	sealed class RuntimeInternalFrame : RuntimeFrame
	{
		public RuntimeInternalFrame(CorDebugInternalFrameType internalFrameType)
		{
			InternalFrameType = internalFrameType;
		}

		public CorDebugInternalFrameType InternalFrameType { get; }

		public override void Apply(IRuntimeFrameVisitor visitor) => visitor.Visit(this);
	}
}
