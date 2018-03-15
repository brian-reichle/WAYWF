// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WAYWF.Agent.CorDebugApi;
using WAYWF.Agent.MetaDataApi;

namespace WAYWF.Agent
{
	static class MetaDataImportExtensions
	{
		public static unsafe void GetScopeProps(this IMetaDataImport import, out string name, out Guid mvid)
		{
			int size;

			fixed (Guid* ptr = &mvid)
			{
				import.GetScopeProps(
					pchName: &size,
					pmvid: ptr);
			}

			if (size <= 1)
			{
				name = string.Empty;
			}
			else
			{
				var buffer = stackalloc char[size];

				import.GetScopeProps(
					szName: buffer,
					cchName: size,
					pchName: &size);

				name = new string(buffer, 0, size - 1);
			}
		}

		public static unsafe void GetTypeDefProps(this IMetaDataImport import, MetaDataToken td, out string name, out MetaDataToken declaringType, out MetaDataToken baseType)
		{
			int size;
			TypeAttributes flags;

			fixed (MetaDataToken* ptr = &baseType)
			{
				import.GetTypeDefProps(
					td,
					pchTypeDef: &size,
					pdwTypeDefFlags: &flags,
					ptkExtends: ptr);
			}

			if (size <= 1)
			{
				name = string.Empty;
			}
			else
			{
				var buffer = stackalloc char[size];

				import.GetTypeDefProps(
					td,
					szTypeDef: buffer,
					cchTypeDef: size,
					pchTypeDef: &size);

				name = new string(buffer, 0, size - 1);
			}

			if (flags.IsNestedType())
			{
				import.GetNestedClassProps(td, out declaringType);
			}
			else
			{
				declaringType = MetaDataToken.Nil;
			}
		}

		public static unsafe void GetMethodProps(this IMetaDataImport import, MetaDataToken mb, out MetaDataToken cl, out string name, out IntPtr sigPtr, out int sigLen, out int rva)
		{
			int size;

			fixed (MetaDataToken* clPtr = &cl)
			fixed (IntPtr* sigPtrPtr = &sigPtr)
			fixed (int* sigLenPtr = &sigLen)
			fixed (int* rvaPtr = &rva)
			{
				import.GetMethodProps(
					mb,
					pClass: clPtr,
					pchMethod: &size,
					ppvSigBlob: sigPtrPtr,
					pcbSigBlob: sigLenPtr,
					pulCodeRVA: rvaPtr);
			}

			if (size <= 1)
			{
				name = string.Empty;
			}
			else
			{
				var buffer = stackalloc char[size];

				import.GetMethodProps(
					mb,
					szMethod: buffer,
					cchMethod: size,
					pchMethod: &size);

				name = new string(buffer, 0, size - 1);
			}
		}

		public static unsafe void GetTypeRefProps(this IMetaDataImport import, MetaDataToken tr, out MetaDataToken scope, out string className)
		{
			int size;

			fixed (MetaDataToken* scopePtr = &scope)
			{
				import.GetTypeRefProps(
					tr,
					ptkResolutionScope: scopePtr,
					pchName: &size);
			}

			if (size <= 1)
			{
				className = string.Empty;
			}
			else
			{
				var buffer = stackalloc char[size];

				import.GetTypeRefProps(
					tr,
					szName: buffer,
					cchName: size,
					pchName: &size);

				className = new string(buffer, 0, size - 1);
			}
		}

		public static unsafe MetaDataToken[] GetFields(this IMetaDataImport import, MetaDataToken td)
		{
			var hEnum = IntPtr.Zero;

			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				import.EnumFields(ref hEnum, td, null, 0);

				var count = import.CountEnum(hEnum);

				if (count == 0)
				{
					return EmptyCollections<MetaDataToken>.EmptyArray;
				}

				var result = new MetaDataToken[count];

				fixed (MetaDataToken* ptr = &result[0])
				{
					count = import.EnumFields(ref hEnum, td, ptr, count);
				}

				if (count != result.Length)
				{
					Array.Resize(ref result, count);
				}

				return result;
			}
			finally
			{
				import.CloseEnum(hEnum);
			}
		}

		public static unsafe void GetFieldProps(this IMetaDataImport import, MetaDataToken mb, out string name, out FieldAttributes att, out CorElementType type, out IntPtr valuePtr, out int valueLen)
		{
			int size;
			IntPtr sigPtr;
			int sigLen;

			fixed (FieldAttributes* ptr = &att)
			fixed (IntPtr* valuePtrPtr = &valuePtr)
			fixed (int* valueLenPtr = &valueLen)
			{
				import.GetFieldProps(
					mb,
					pchField: &size,
					pdwAttr: ptr,
					ppvSigBlob: &sigPtr,
					pcbSigBlob: &sigLen,
					ppValue: valuePtrPtr,
					pcchValue: valueLenPtr);
			}

			type = DistilFieldType(sigPtr, sigLen);

			if (size <= 1)
			{
				name = string.Empty;
			}
			else
			{
				var buffer = stackalloc char[size];

				import.GetFieldProps(
					mb,
					szField: buffer,
					cchField: size);

				name = new string(buffer, 0, size - 1);
			}
		}

		public static unsafe void GetFieldProps(this IMetaDataImport import, MetaDataToken mb, out string name, out IntPtr sigPtr, out int sigLen)
		{
			int size;

			fixed (IntPtr* sigPtrPtr = &sigPtr)
			fixed (int* sigLenPtr = &sigLen)
			{
				import.GetFieldProps(
					mb,
					pchField: &size,
					ppvSigBlob: sigPtrPtr,
					pcbSigBlob: sigLenPtr);
			}

			if (size <= 1)
			{
				name = string.Empty;
			}
			else
			{
				var buffer = stackalloc char[size];

				import.GetFieldProps(
					mb,
					szField: buffer,
					cchField: size);

				name = new string(buffer, 0, size - 1);
			}
		}

		public static unsafe void GetFieldProps(this IMetaDataImport import, MetaDataToken fieldToken, out MetaDataToken classToken, out string name)
		{
			int size;

			fixed (MetaDataToken* classTokenPtr = &classToken)
			{
				import.GetFieldProps(fieldToken, pClass: classTokenPtr, pchField: &size);
			}

			if (size <= 1)
			{
				name = string.Empty;
			}

			var buffer = stackalloc char[size];
			import.GetFieldProps(fieldToken, szField: buffer, cchField: size);
			name = new string(buffer, 0, size - 1);
		}

		public static unsafe void GetFieldProps(this IMetaDataImport import, MetaDataToken mb, out CorElementType type)
		{
			IntPtr sigPtr;
			int sigLen;

			import.GetFieldProps(
				mb,
				ppvSigBlob: &sigPtr,
				pcbSigBlob: &sigLen);

			type = DistilFieldType(sigPtr, sigLen);
		}

		public static unsafe void GetMemberRefProps(this IMetaDataImport import, MetaDataToken mr, out MetaDataToken classToken, out string name)
		{
			int size;

			fixed (MetaDataToken* classTokenPtr = &classToken)
			{
				import.GetMemberRefProps(mr, ptk: classTokenPtr, pchMember: &size);
			}

			if (size <= 1)
			{
				name = string.Empty;
				return;
			}

			var buffer = stackalloc char[size];
			import.GetMemberRefProps(mr, szMember: buffer, cchMember: size);
			name = new string(buffer, 0, size - 1);
		}

		public static unsafe string GetParamName(this IMetaDataImport import, MetaDataToken tk)
		{
			int size;

			import.GetParamProps(
				tk,
				pchName: &size);

			if (size <= 1)
			{
				return string.Empty;
			}

			var buffer = stackalloc char[size];

			import.GetParamProps(
				tk,
				szName: buffer,
				cchName: size,
				pchName: &size);

			return new string(buffer, 0, size - 1);
		}

		public static unsafe MetaDataToken GetCustomAttributeType(this IMetaDataImport import, MetaDataToken cv)
		{
			MetaDataToken constructorToken;
			MetaDataToken typeToken;

			import.GetCustomAttributeProps(
				cv,
				ptkType: &constructorToken);

			if (constructorToken.TokenType == TokenType.MethodDef)
			{
				import.GetMethodProps(
					constructorToken,
					pClass: &typeToken);
			}
			else if (constructorToken.TokenType == TokenType.MemberRef)
			{
				import.GetMemberRefProps(
					constructorToken,
					ptk: &typeToken);
			}
			else
			{
				typeToken = MetaDataToken.Nil;
			}

			return typeToken;
		}

		public static string GetParamName(this IMetaDataImport import, MetaDataToken methodToken, int index)
		{
			var hr = import.GetParamForMethodIndex(methodToken, index, out var paramToken);

			if (hr == HResults.CLDB_E_RECORD_NOTFOUND)
			{
				return string.Empty;
			}
			else if (hr < 0)
			{
				throw Marshal.GetExceptionForHR(hr);
			}

			return import.GetParamName(paramToken);
		}

		public static unsafe int GetTypeArgCount(this IMetaDataImport import, MetaDataToken tk)
		{
			var import2 = (IMetaDataImport2)import;
			var hEnum = IntPtr.Zero;

			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				if (!import2.EnumGenericParams(ref hEnum, tk, out var paramToken))
				{
					return 0;
				}

				var result = import2.CountEnum(hEnum);
				return result;
			}
			finally
			{
				if (hEnum != IntPtr.Zero)
				{
					import2.CloseEnum(hEnum);
				}
			}
		}

		static unsafe CorElementType DistilFieldType(IntPtr sigPtr, int sigLen)
		{
			const byte FIELD = 0x06;

			if (sigLen <= 0) throw new InvalidSignatureException();

			var ptr = (byte*)sigPtr;
			var end = ptr + sigLen;
			if (*(ptr++) != FIELD) throw new InvalidSignatureException();

			while (true)
			{
				if (ptr >= end) throw new InvalidSignatureException();

				var type = (CorElementType)(*ptr++);

				if (type != CorElementType.ELEMENT_TYPE_CMOD_OPT && type != CorElementType.ELEMENT_TYPE_CMOD_REQD)
				{
					return type;
				}

				if (ptr >= end) throw new InvalidSignatureException();
				var lead = *(ptr++);

				if (lead >= 0x80)
				{
					if (lead < 0xC0)
					{
						ptr++;
					}
					else if (lead < 0xE0)
					{
						ptr += 3;
					}
				}
			}
		}
	}
}
