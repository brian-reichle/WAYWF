// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace WAYWF.Agent
{
	[DebuggerTypeProxy(typeof(TypeProxy))]
	unsafe class BlobReader
	{
		public BlobReader(IntPtr buffer, int length)
			: this((byte*)buffer, length)
		{
		}

		public BlobReader(byte* buffer, int length)
		{
			_buffer = _start = buffer;
			_end = buffer + length;
		}

		public byte PeekByte()
		{
			if (_buffer >= _end)
			{
				throw new InvalidSignatureException();
			}

			return *(_buffer);
		}

		public byte ReadByte()
		{
			if (_buffer >= _end)
			{
				throw new InvalidSignatureException();
			}

			return *(_buffer++);
		}

		public ushort ReadUInt16()
		{
			if (_buffer + 1 >= _end)
			{
				throw new InvalidSignatureException();
			}

			var result = *((ushort*)_buffer);
			_buffer += 2;
			return result;
		}

		public int ReadCompressedUInt()
		{
			if (_buffer >= _end)
			{
				throw new InvalidSignatureException();
			}

			int tmp = *(_buffer++);

			if ((tmp & 0x80) == 0)
			{
			}
			else if ((tmp & 0xC0) == 0x80)
			{
				if (_buffer >= _end)
				{
					throw new InvalidSignatureException();
				}

				tmp = ((tmp & 0x3F) << 8) | *_buffer;
				_buffer++;
			}
			else if ((tmp & 0xE0) == 0xC0)
			{
				if (_buffer + 2 >= _end)
				{
					throw new InvalidSignatureException();
				}

				tmp = ((tmp & 0x1f) << 8) | *(_buffer++);
				tmp = (tmp << 8) | *(_buffer++);
				tmp = (tmp << 8) | *(_buffer++);
			}
			else
			{
				tmp = 0;
			}

			return tmp;
		}

		public int ReadCompressedInt()
		{
			if (_buffer >= _end)
			{
				throw new InvalidSignatureException();
			}

			int tmp = *(_buffer++);
			int mask;

			if ((tmp & 0x80) == 0)
			{
				mask = unchecked((int)0xFFFFFFC0);
			}
			else if ((tmp & 0xC0) == 0x80)
			{
				if (_buffer >= _end)
				{
					throw new InvalidSignatureException();
				}

				tmp = ((tmp & 0x3F) << 8) | *(_buffer++);
				mask = unchecked((int)0xFFFFE000);
			}
			else if ((tmp & 0xE0) == 0xC0)
			{
				if (_buffer + 2 >= _end)
				{
					throw new InvalidSignatureException();
				}

				tmp = ((tmp & 0x1f) << 8) | *(_buffer++);
				tmp = (tmp << 8) | *(_buffer++);
				tmp = (tmp << 8) | *(_buffer++);
				mask = unchecked((int)0xF0000000);
			}
			else
			{
				tmp = 0;
				mask = 0;
			}

			if ((tmp & 1) == 0)
			{
				mask = 0;
			}

			tmp = (tmp >> 1) | mask;

			return tmp;
		}

		public string ReadSerString()
		{
			if (PeekByte() == 0xFF)
			{
				_buffer++;
				return null;
			}

			var length = ReadCompressedUInt();

			if (length == 0)
			{
				return string.Empty;
			}

			var chars = new char[length];

			var decoder = Encoding.UTF8.GetDecoder();
			int bytesUsed;

			fixed (char* charsPtr = &chars[0])
			{
				decoder.Convert(_buffer, (int)(_end - _buffer), charsPtr, length, true, out bytesUsed, out var charsUsed, out var completed);
			}

			_buffer += bytesUsed;

			return new string(chars);
		}

		public MetaDataToken ReadTypeDefOrRefOrSpecEncoded()
		{
			var val = ReadCompressedUInt();

			TokenType type;

			switch (val & 0x03)
			{
				case 0:
					type = TokenType.TypeDef;
					break;

				case 1:
					type = TokenType.TypeRef;
					break;

				case 2:
					type = TokenType.TypeSpec;
					break;

				default:
					throw new InvalidSignatureException();
			}

			return new MetaDataToken(unchecked((int)type) | val >> 2);
		}

		public bool EOF => _buffer >= _end;

		byte* _buffer;
		readonly byte* _start;
		readonly byte* _end;

		sealed class TypeProxy
		{
			public TypeProxy(BlobReader reader)
			{
				Count = (int)(reader._end - reader._start);
				Position = (int)(reader._buffer - reader._start);
				_data = new byte[Count];
				Marshal.Copy((IntPtr)reader._start, _data, 0, Count);
			}

			public int Position { get; }
			public int Count { get; }

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			readonly byte[] _data;
		}
	}
}
