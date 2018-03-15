// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Threading;

namespace WAYWF.Agent.IL
{
	partial class ILReader : IEnumerator<Instruction>, IEnumerable<Instruction>
	{
		public ILReader(byte[] buffer)
		{
			_buffer = buffer;
			_offset = 0;
		}

		public IEnumerator<Instruction> GetEnumerator()
		{
			if (Interlocked.Exchange(ref _started, 1) == 0)
			{
				return this;
			}
			else
			{
				return new ILReader(_buffer);
			}
		}

		protected bool MoveNext()
		{
			if (_offset == _buffer.Length)
			{
				_current = null;
				return false;
			}
			else
			{
				_current = Read();
				return true;
			}
		}

		#region IEnumerable Members

		[DebuggerStepThrough]
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		#endregion

		#region IEnumerator<Instruction> Members

		Instruction IEnumerator<Instruction>.Current => _current;

		#endregion

		#region IDisposable Members

		void IDisposable.Dispose()
		{
		}

		#endregion

		#region IEnumerator Members

		object IEnumerator.Current => _current;

		[DebuggerStepThrough]
		bool IEnumerator.MoveNext() => MoveNext();

		void IEnumerator.Reset()
		{
			_offset = 0;
			_current = null;
		}

		#endregion

		Instruction ReadInlineNoneInstruction(OpCode opcode)
		{
			var result = new Instruction(_offset, opcode);
			_offset += opcode.Size;
			return result;
		}

		Instruction ReadShortInlineVarInstruction(OpCode opcode)
		{
			return CreateInstruction(ref _offset, opcode.Size + 1, opcode, (short)ReadUInt8(_buffer, _offset + opcode.Size));
		}

		Instruction ReadShortInlineIInstruction(OpCode opcode)
		{
			return CreateInstruction<int>(ref _offset, opcode.Size + 1, opcode, (sbyte)ReadUInt8(_buffer, _offset + opcode.Size));
		}

		Instruction ReadInlineIInstruction(OpCode opcode)
		{
			return CreateInstruction(ref _offset, opcode.Size + 4, opcode, ReadInt32(_buffer, _offset + opcode.Size));
		}

		Instruction ReadInlineI8Instruction(OpCode opcode)
		{
			return CreateInstruction(ref _offset, opcode.Size + 8, opcode, ReadInt64(_buffer, _offset + opcode.Size));
		}

		Instruction ReadShortInlineRInstruction(OpCode opcode)
		{
			return CreateInstruction(ref _offset, opcode.Size + 4, opcode, ReadSingle(_buffer, _offset + opcode.Size));
		}

		Instruction ReadInlineRInstruction(OpCode opcode)
		{
			return CreateInstruction(ref _offset, opcode.Size + 8, opcode, ReadDouble(_buffer, _offset + opcode.Size));
		}

		Instruction ReadInlineTokInstruction(OpCode opcode)
		{
			return CreateInstruction(ref _offset, opcode.Size + 4, opcode, new MetaDataToken(ReadInt32(_buffer, _offset + opcode.Size)));
		}

		Instruction ReadInlineVarInstruction(OpCode opcode)
		{
			return CreateInstruction(ref _offset, opcode.Size + 2, opcode, ReadInt16(_buffer, _offset + opcode.Size));
		}

		Instruction ReadShortInlineBrTargetInstruction(OpCode opcode)
		{
			var delta = (sbyte)ReadUInt8(_buffer, _offset + opcode.Size);
			var length = opcode.Size + 1;
			return CreateInstruction(ref _offset, length, opcode, _offset + length + delta);
		}

		Instruction ReadInlineBrTargetInstruction(OpCode opcode)
		{
			var delta = ReadInt32(_buffer, _offset + opcode.Size);
			var length = opcode.Size + 4;
			return CreateInstruction(ref _offset, length, opcode, _offset + length + delta);
		}

		Instruction ReadInlineSwitchInstruction(OpCode opcode)
		{
			var pos = _offset + opcode.Size;
			var labelCount = ReadInt32(_buffer, pos);

			pos += 4;
			var relativeOffset = pos + labelCount * 4;

			var value = new int[labelCount];

			for (var i = 0; i < value.Length; i++)
			{
				value[i] = relativeOffset + ReadInt32(_buffer, pos);
				pos += 4;
			}

			return CreateInstruction(ref _offset, pos - _offset, opcode, value);
		}

		static Instruction<T> CreateInstruction<T>(ref int offset, int length, OpCode opcode, T value)
		{
			var result = new Instruction<T>(offset, opcode, value);
			offset += length;
			return result;
		}

		static byte ReadUInt8(byte[] buffer, int offset)
		{
			if (offset + 1 >= buffer.Length)
			{
				throw new ILException("Incomplete Argument");
			}

			return buffer[offset];
		}

		static short ReadInt16(byte[] buffer, int offset)
		{
			if (offset + 2 >= buffer.Length)
			{
				throw new ILException("Incomplete Argument");
			}

			return BitConverter.ToInt16(buffer, offset);
		}

		static int ReadInt32(byte[] buffer, int offset)
		{
			if (offset + 4 >= buffer.Length)
			{
				throw new ILException("Incomplete Argument");
			}

			return BitConverter.ToInt32(buffer, offset);
		}

		static long ReadInt64(byte[] buffer, int offset)
		{
			if (offset + 8 >= buffer.Length)
			{
				throw new ILException("Incomplete Argument");
			}

			return BitConverter.ToInt64(buffer, offset);
		}

		static float ReadSingle(byte[] buffer, int offset)
		{
			if (offset + 4 >= buffer.Length)
			{
				throw new ILException("Incomplete Argument");
			}

			return BitConverter.ToSingle(buffer, offset);
		}

		static double ReadDouble(byte[] buffer, int offset)
		{
			if (offset + 8 >= buffer.Length)
			{
				throw new ILException("Incomplete Argument");
			}

			return BitConverter.ToDouble(buffer, offset);
		}

		readonly byte[] _buffer;
		int _started;
		int _offset;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Instruction _current;
	}
}
