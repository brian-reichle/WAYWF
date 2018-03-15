// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WAYWF.Agent.CorDebugApi;
using WAYWF.Agent.IL;
using WAYWF.Agent.MetaDataApi;
using WAYWF.Agent.SymbolStoreApi;

namespace WAYWF.Agent
{
	static class ModuleExtensions
	{
		public static unsafe string GetName(this ICorDebugModule module)
		{
			module.GetName(0, out var size, null);

			if (size <= 1)
			{
				return string.Empty;
			}

			var buffer = stackalloc char[size];
			module.GetName(size, out size, buffer);
			return new string(buffer, 0, size - 1);
		}

		public static IMetaDataImport GetMetaDataImport(this ICorDebugModule module)
		{
			return (IMetaDataImport)module.GetMetaDataInterface(typeof(IMetaDataImport).GUID);
		}

		public static IMetaDataAssemblyImport GetMetaDataAssemblyImport(this ICorDebugModule module)
		{
			return (IMetaDataAssemblyImport)module.GetMetaDataInterface(typeof(IMetaDataAssemblyImport).GUID);
		}

		public static ISymUnmanagedReader CreateReaderForInMemorySymbols(this ICorDebugModule3 module)
		{
			var hr = module.CreateReaderForInMemorySymbols(typeof(ISymUnmanagedReader).GUID, out var reader);

			if (hr == HResults.CORDBG_E_SYMBOLS_NOT_AVAILABLE)
			{
				return null;
			}
			else if (hr < 0)
			{
				throw Marshal.GetExceptionForHR(hr);
			}

			return (ISymUnmanagedReader)reader;
		}

		public static bool IsEnum(this ICorDebugModule module, MetaDataToken token)
		{
			return module.IsType(token, "mscorlib", "System.Enum");
		}

		public static bool HasFlagsAttribute(this ICorDebugModule module, MetaDataToken token)
		{
			return HasAttribute(module, token, "mscorlib", "System.FlagsAttribute");
		}

		public static ICorDebugAssembly ResolveAssembly(this ICorDebugModule module, MetaDataToken token)
		{
			var hr = ((ICorDebugModule2)module).ResolveAssembly(token, out var assembly);

			if (hr == 0)
			{
				return assembly;
			}
			else if (hr == HResults.CORDBG_E_CANNOT_RESOLVE_ASSEMBLY)
			{
				return null;
			}
			else
			{
				throw Marshal.GetExceptionForHR(hr);
			}
		}

		public static unsafe bool IsStateMachineMatching(this ICorDebugModule module, MetaDataToken methodToken, string smTypeName)
		{
			var e = IntPtr.Zero;
			var import = module.GetMetaDataImport();
			var result = false;

			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				MetaDataToken constructorToken;

				while (import.EnumCustomAttributes(ref e, methodToken, MetaDataToken.Nil, out var attToken))
				{
					import.GetCustomAttributeProps(attToken, ptkType: &constructorToken);

					if (!module.IsMethodOf(constructorToken, "mscorlib", "System.Runtime.CompilerServices.AsyncStateMachineAttribute"))
					{
						continue;
					}

					IntPtr blobPtr;
					int blobSize;

					import.GetCustomAttributeProps(attToken, ppBlob: &blobPtr, pcbSize: &blobSize);

					if (ReadSingleTypeArgument(blobPtr, blobSize) == smTypeName)
					{
						result = true;
						break;
					}
				}
			}
			finally
			{
				if (e != IntPtr.Zero)
				{
					import.CloseEnum(e);
				}
			}

			return result;
		}

		public static unsafe bool IsMethodOf(this ICorDebugModule module, MetaDataToken memberToken, string assemblyName, string typeName)
		{
			if (memberToken.IsNil)
			{
				return false;
			}
			else if (memberToken.TokenType == TokenType.MethodDef)
			{
				var import = module.GetMetaDataImport();

				MetaDataToken typeToken;
				import.GetMethodProps(memberToken, pClass: &typeToken);

				return IsType(module, typeToken, assemblyName, typeName);
			}
			else if (memberToken.TokenType == TokenType.MemberRef)
			{
				var import = module.GetMetaDataImport();

				MetaDataToken typeToken;

				import.GetMemberRefProps(memberToken, ptk: &typeToken);

				return IsType(module, typeToken, assemblyName, typeName);
			}
			else
			{
				return false;
			}
		}

		public static unsafe bool IsMethod(this ICorDebugModule module, MetaDataToken memberToken, string assemblyName, string typeName, string methodName)
		{
			if (memberToken.IsNil)
			{
				return false;
			}
			else if (memberToken.TokenType == TokenType.MethodDef)
			{
				var import = module.GetMetaDataImport();

				MetaDataToken typeToken;
				int size;
				import.GetMethodProps(memberToken, pClass: &typeToken, pchMethod: &size);

				if (size != methodName.Length + 1 || !IsType(module, typeToken, assemblyName, typeName))
				{
					return false;
				}

				var buffer = stackalloc char[size];

				import.GetMethodProps(memberToken, szMethod: buffer, cchMethod: size);
				return IsMatch(methodName, buffer, size);
			}
			else if (memberToken.TokenType == TokenType.MemberRef)
			{
				var import = module.GetMetaDataImport();

				MetaDataToken typeToken;
				int size;

				import.GetMemberRefProps(memberToken, ptk: &typeToken, pchMember: &size);

				if (size != methodName.Length + 1 || !IsType(module, typeToken, assemblyName, typeName))
				{
					return false;
				}

				var buffer = stackalloc char[size];

				import.GetMemberRefProps(memberToken, szMember: buffer, cchMember: size);
				return IsMatch(methodName, buffer, size);
			}
			else
			{
				return false;
			}
		}

		public static unsafe bool IsType(this ICorDebugModule module, MetaDataToken token, string assemblyName, string typeName)
		{
			if (token.IsNil)
			{
				return false;
			}
			else if (token.TokenType == TokenType.TypeDef)
			{
				int size;
				TypeAttributes att;

				var import = module.GetMetaDataImport();

				import.GetTypeDefProps(
					token,
					pchTypeDef: &size,
					pdwTypeDefFlags: &att);

				if (size != typeName.Length + 1 || att.IsNestedType() || !MatchesAssemblyName(module, MetaDataToken.Module, assemblyName))
				{
					return false;
				}

				var buffer = stackalloc char[size];

				import.GetTypeDefProps(
					token,
					szTypeDef: buffer,
					cchTypeDef: size);

				return IsMatch(typeName, buffer, size);
			}
			else if (token.TokenType == TokenType.TypeRef)
			{
				int size;
				MetaDataToken scope;

				var import = module.GetMetaDataImport();

				import.GetTypeRefProps(
					token,
					ptkResolutionScope: &scope,
					pchName: &size);

				if (size != typeName.Length + 1 || !MatchesAssemblyName(module, scope, assemblyName))
				{
					return false;
				}

				var buffer = stackalloc char[size];

				import.GetTypeRefProps(
					token,
					szName: buffer,
					cchName: size);

				return IsMatch(typeName, buffer, size);
			}
			else
			{
				return false;
			}
		}

		public static unsafe IEnumerable<Instruction> GetMethodIL(this ICorDebugModule module, MetaDataToken mb)
		{
			var import = module.GetMetaDataImport();
			int rva;

			import.GetMethodProps(mb, pulCodeRVA: &rva);
			if (rva == 0) return null;

			var methodAddress = module.GetBaseAddress() + rva;
			var il = module.GetMethodILBytes(methodAddress);
			return new ILReader(il);
		}

		public static MetaDataToken GetMethodBody(this ICorDebugModule module, MetaDataToken type, string assemblyName, string interfaceName, string methodName)
		{
			var hEnum = IntPtr.Zero;
			var import = module.GetMetaDataImport();

			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				while (import.EnumMethodImpls(ref hEnum, type, out var bodyToken, out var declToken))
				{
					if (IsMethod(module, declToken, assemblyName, interfaceName, methodName))
					{
						return bodyToken;
					}
				}
			}
			finally
			{
				if (hEnum != IntPtr.Zero)
				{
					import.CloseEnum(hEnum);
				}
			}

			return MetaDataToken.Nil;
		}

		static byte[] GetMethodILBytes(this ICorDebugModule module, CORDB_ADDRESS methodAddress)
		{
			var process = module.GetProcess();
			var firstInt = process.ReadInt(methodAddress);

			byte[] result;

			if ((process.ReadInt(methodAddress) & 0x03) != 0x03)
			{
				var length = firstInt >> 2 & 0x1F;
				result = process.ReadBytes(methodAddress + 1, length);
			}
			else
			{
				var headerLength = firstInt >> 10 & 0x3C;
				var codeSize = process.ReadInt(methodAddress + 4);
				result = process.ReadBytes(methodAddress + headerLength, codeSize);
			}

			return result;
		}

		static string ReadSingleTypeArgument(IntPtr blobPtr, int blobSize)
		{
			var reader = new BlobReader(blobPtr, blobSize);

			if (reader.ReadUInt16() != 1)
			{
				throw new InvalidSignatureException();
			}

			var typeName = reader.ReadSerString();

			if (reader.ReadUInt16() != 0 || !reader.EOF)
			{
				throw new InvalidSignatureException();
			}

			return typeName;
		}

		static bool HasAttribute(ICorDebugModule module, MetaDataToken token, string assemblyName, string typeName)
		{
			var import = module.GetMetaDataImport();
			var hEnum = IntPtr.Zero;

			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				while (import.EnumCustomAttributes(ref hEnum, token, MetaDataToken.Nil, out var caToken))
				{
					var type = import.GetCustomAttributeType(caToken);

					if (module.IsType(type, assemblyName, typeName))
					{
						return true;
					}
				}
			}
			finally
			{
				if (hEnum != IntPtr.Zero)
				{
					import.CloseEnum(hEnum);
				}
			}

			return false;
		}

		static unsafe bool MatchesAssemblyName(ICorDebugModule module, MetaDataToken token, string assemblyName)
		{
			if (token.IsNil)
			{
				return false;
			}
			else if (token == MetaDataToken.Module || token.TokenType == TokenType.ModuleRef)
			{
				if (module.IsInMemory())
				{
					return false;
				}

				var aImport = module.GetMetaDataAssemblyImport();

				int size;
				var hr = aImport.GetAssemblyFromScope(out var assemblyToken);

				if (hr < 0)
				{
					return false;
				}

				aImport.GetAssemblyProps(
					assemblyToken,
					pchName: &size);

				if (size != assemblyName.Length + 1)
				{
					return false;
				}

				var buffer = stackalloc char[assemblyName.Length + 1];

				aImport.GetAssemblyProps(
					assemblyToken,
					szName: buffer,
					cchName: assemblyName.Length + 1);

				return IsMatch(assemblyName, buffer, size);
			}
			else if (token.TokenType == TokenType.AssemblyRef)
			{
				int size;

				var aImport = module.GetMetaDataAssemblyImport();

				aImport.GetAssemblyRefProps(
					token,
					pchName: &size);

				if (size != assemblyName.Length + 1)
				{
					return false;
				}

				var buffer = stackalloc char[assemblyName.Length + 1];

				aImport.GetAssemblyRefProps(
					token,
					szName: buffer,
					cchName: assemblyName.Length + 1);

				return IsMatch(assemblyName, buffer, size);
			}
			else
			{
				return false;
			}
		}

		static unsafe bool IsMatch(string str, char* buffer, int len)
		{
			if (len != str.Length + 1)
			{
				return false;
			}

			for (var i = 0; i < str.Length; i++)
			{
				if (str[i] != *buffer)
				{
					return false;
				}

				buffer++;
			}

			return *buffer == '\0';
		}
	}
}
