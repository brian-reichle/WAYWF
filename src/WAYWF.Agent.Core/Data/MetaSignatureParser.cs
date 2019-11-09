// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Reflection;
using WAYWF.Agent.Core.CorDebugApi;
using WAYWF.Agent.Data;

namespace WAYWF.Agent.Core
{
	static class MetaSignatureParser
	{
		public static MetaMethodSignature ReadMethodDefSig(ISignatureContext context, IntPtr sigPtr, int sigLen)
		{
			var sig = new Signature(context, sigPtr, sigLen);
			var result = ReadMethodSig(sig, false, true);
			if (!sig.EOF) throw new InvalidSignatureException();
			return result;
		}

		public static MetaVariable[] ReadLocalSig(ISignatureContext context, IntPtr sigPtr, int sigLen)
		{
			var sig = new Signature(context, sigPtr, sigLen);
			var result = ReadLocalSig(sig);
			if (!sig.EOF) throw new InvalidSignatureException();
			return result;
		}

		public static MetaTypeBase ReadTypeSig(ISignatureContext context, IntPtr sigPtr, int sigLen)
		{
			var sig = new Signature(context, sigPtr, sigLen);
			var result = ReadTypeCore(sig);
			if (!sig.EOF) throw new InvalidSignatureException();
			return result;
		}

		public static MetaTypeBase ReadFieldType(ISignatureContext context, IntPtr sigPtr, int sigLen)
		{
			var sig = new Signature(context, sigPtr, sigLen);
			var result = ReadFieldType(sig);
			if (!sig.EOF) throw new InvalidSignatureException();
			return result;
		}

		static MetaMethodSignature ReadMethodSig(Signature sig, bool allowSentinel, bool isTopLevel)
		{
			const CallingConventions mask =
				CallingConventions.HasThis |
				CallingConventions.ExplicitThis;

			const byte Default = 0x00;
			const byte VarArgs = 0x05;
			const byte Generic = 0x10;

			var preamble = sig.ReadByte();
			var genParamCount = 0;
			CallingConventions callingConvention;

			switch (unchecked((byte)~mask & preamble))
			{
				case VarArgs:
					callingConvention = CallingConventions.VarArgs;
					break;

				case Generic:
					genParamCount = sig.ReadCompressedUInt();
					goto case Default;

				case Default:
					callingConvention = CallingConventions.Standard;
					allowSentinel = false;
					break;

				default:
					throw new InvalidSignatureException();
			}

			callingConvention |= mask & (CallingConventions)preamble;

			var paramCount = sig.ReadCompressedUInt();
			var parameters = new MetaVariable[paramCount];
			var resultParam = ReadParameter(sig, null);

			for (var i = 0; i < parameters.Length; i++)
			{
				if ((CorElementType)sig.PeekByte() == CorElementType.ELEMENT_TYPE_SENTINEL)
				{
					if (!allowSentinel) throw new InvalidSignatureException();
					sig.ReadByte();
					allowSentinel = false;
				}

				parameters[i] = ReadParameter(sig, isTopLevel ? i + 1 : default(int?));
			}

			return new MetaMethodSignature(callingConvention, genParamCount, resultParam, parameters);
		}

		static MetaVariable[] ReadLocalSig(Signature sig)
		{
			if (sig.ReadByte() != LOCAL_SIG) throw new InvalidSignatureException();

			var count = sig.ReadCompressedUInt();
			var locals = new MetaVariable[count];

			for (var i = 0; i < locals.Length; i++)
			{
				locals[i] = ReadLocal(sig, i);
			}

			return locals;
		}

		static MetaTypeBase ReadFieldType(Signature sig)
		{
			if (sig.ReadByte() != FIELD_SIG) throw new InvalidSignatureException();
			SkipCMODList(sig);
			return ReadTypeCore(sig);
		}

		static MetaTypeBase ReadTypeCore(Signature sig)
		{
			var elementType = (CorElementType)sig.ReadByte();

			switch (elementType)
			{
				case CorElementType.ELEMENT_TYPE_BOOLEAN:
				case CorElementType.ELEMENT_TYPE_CHAR:
				case CorElementType.ELEMENT_TYPE_I1:
				case CorElementType.ELEMENT_TYPE_U1:
				case CorElementType.ELEMENT_TYPE_I2:
				case CorElementType.ELEMENT_TYPE_U2:
				case CorElementType.ELEMENT_TYPE_I4:
				case CorElementType.ELEMENT_TYPE_U4:
				case CorElementType.ELEMENT_TYPE_I8:
				case CorElementType.ELEMENT_TYPE_U8:
				case CorElementType.ELEMENT_TYPE_R4:
				case CorElementType.ELEMENT_TYPE_R8:
				case CorElementType.ELEMENT_TYPE_I:
				case CorElementType.ELEMENT_TYPE_U:
				case CorElementType.ELEMENT_TYPE_OBJECT:
				case CorElementType.ELEMENT_TYPE_STRING:
				case CorElementType.ELEMENT_TYPE_VOID:
					return MetaDataCache.GetType(elementType);

				case CorElementType.ELEMENT_TYPE_ARRAY:
					return ReadArray(sig);

				case CorElementType.ELEMENT_TYPE_CLASS:
				case CorElementType.ELEMENT_TYPE_VALUETYPE:
					return ReadClass(sig);

				case CorElementType.ELEMENT_TYPE_MVAR:
					return ReadVar(true, sig);

				case CorElementType.ELEMENT_TYPE_VAR:
					return ReadVar(false, sig);

				case CorElementType.ELEMENT_TYPE_PTR:
					return ReadPointer(sig);

				case CorElementType.ELEMENT_TYPE_GENERICINST:
					return ReadGenericInst(sig);

				case CorElementType.ELEMENT_TYPE_SZARRAY:
					return ReadSZArray(sig);

				case CorElementType.ELEMENT_TYPE_FNPTR:
					return ReadFunctionPTR(sig);

				default:
					throw new InvalidSignatureException();
			}
		}

		static MetaArrayType ReadArray(Signature sig)
		{
			var baseType = ReadTypeCore(sig);
			var rank = sig.ReadCompressedUInt();
			var numSizes = sig.ReadCompressedUInt();

			for (var i = 0; i < numSizes; i++)
			{
				sig.ReadCompressedUInt();
			}

			var numLoBounds = sig.ReadCompressedUInt();

			for (var i = 0; i < numLoBounds; i++)
			{
				sig.ReadCompressedInt();
			}

			return new MetaArrayType(baseType, rank);
		}

		static MetaArrayType ReadSZArray(Signature sig)
		{
			SkipCMODList(sig);
			var baseType = ReadTypeCore(sig);
			return new MetaArrayType(baseType, 1);
		}

		static MetaType ReadClass(Signature sig)
		{
			return sig.Context.GetType(sig.ReadTypeDefOrRefOrSpecEncoded());
		}

		static MetaPointerType ReadPointer(Signature sig)
		{
			SkipCMODList(sig);
			var baseType = ReadTypeCore(sig);
			return new MetaPointerType(baseType);
		}

		static MetaGenType ReadGenericInst(Signature sig)
		{
			sig.ReadByte();
			var baseType = ReadClass(sig);
			var genArgCount = sig.ReadCompressedUInt();
			var genArguments = new MetaTypeBase[genArgCount];

			for (var i = 0; i < genArgCount; i++)
			{
				genArguments[i] = ReadTypeCore(sig);
			}

			return new MetaGenType(baseType, genArguments);
		}

		static MetaType ReadFunctionPTR(Signature sig)
		{
			ReadMethodSig(sig, true, false);
			return MetaKnownType.IntPtr;
		}

		static MetaVarType ReadVar(bool method, Signature sig)
		{
			var genIndex = sig.ReadCompressedUInt();
			return new MetaVarType(method, genIndex);
		}

		static MetaVariable ReadParameter(Signature sig, int? parameterIndex)
		{
			SkipCMODList(sig);
			var byRef = false;
			MetaTypeBase type;

			var elementType = (CorElementType)sig.PeekByte();

			if (elementType == CorElementType.ELEMENT_TYPE_TYPEDBYREF)
			{
				sig.ReadByte();
				type = MetaKnownType.TypedReference;
			}
			else
			{
				if (elementType == CorElementType.ELEMENT_TYPE_BYREF)
				{
					sig.ReadByte();

					// Skip over any custom modifiers between ELEMENT_TYPE_BYREF and the parameter
					// type. The specification does not actually permit custom modifiers here, but
					// Microsofts C++/CLI compiler has a tendency to add them anyway and we don't
					// want to crash due to code that "seems" correct.
					SkipCMODList(sig);
					byRef = true;
				}

				type = ReadTypeCore(sig);
			}

			var name = parameterIndex.HasValue ? sig.Context.GetParamName(parameterIndex.Value) : null;
			return new MetaVariable(type, name, byRef, false);
		}

		static MetaVariable ReadLocal(Signature sig, int localIndex)
		{
			var value = sig.PeekByte();
			MetaTypeBase type;
			var pinned = false;
			var isByRef = false;

			if (value == (byte)CorElementType.ELEMENT_TYPE_TYPEDBYREF)
			{
				sig.ReadByte();
				type = MetaKnownType.TypedReference;
			}
			else
			{
				while (true)
				{
					if (value == (byte)CorElementType.ELEMENT_TYPE_PINNED)
					{
						sig.ReadByte();
						pinned = true;
					}
					else if (value == (byte)CorElementType.ELEMENT_TYPE_CMOD_OPT || value == (byte)CorElementType.ELEMENT_TYPE_CMOD_REQD)
					{
						sig.ReadByte();
						sig.ReadCompressedUInt();
					}
					else if (value == (byte)CorElementType.ELEMENT_TYPE_BYREF)
					{
						sig.ReadByte();
						value = sig.PeekByte();
						isByRef = true;
					}
					else
					{
						break;
					}

					value = sig.PeekByte();
				}

				type = ReadTypeCore(sig);
			}

			var name = sig.Context.GetLocalName(localIndex);
			return new MetaVariable(type, name, isByRef, pinned);
		}

		static void SkipCMODList(Signature sig)
		{
			var elementType = (CorElementType)sig.PeekByte();

			while (elementType == CorElementType.ELEMENT_TYPE_CMOD_OPT || elementType == CorElementType.ELEMENT_TYPE_CMOD_REQD)
			{
				sig.ReadByte();
				sig.ReadCompressedUInt();
				elementType = (CorElementType)sig.PeekByte();
			}
		}

		const int FIELD_SIG = 0x06;
		const int LOCAL_SIG = 0x07;

		sealed class Signature : BlobReader
		{
			public Signature(ISignatureContext context, IntPtr buffer, int length)
				: base(buffer, length)
			{
				Context = context;
			}

			public ISignatureContext Context { get; }
		}
	}
}
