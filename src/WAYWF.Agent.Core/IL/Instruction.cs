// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;
using System.Reflection.Emit;

namespace WAYWF.Agent.Core.IL;

[DebuggerDisplay("{OpCode}")]
class Instruction
{
	public Instruction(int offset, OpCode opCode)
	{
		Offset = offset;
		OpCode = opCode;
	}

	public int Offset { get; }
	public OpCode OpCode { get; }
}
