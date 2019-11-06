// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using WAYWF.Agent.CorDebugApi;
using WAYWF.Agent.MetaDataApi;

namespace WAYWF.Agent.MetaCache
{
	sealed class MetaDataCache
	{
		public MetaDataCache()
		{
			_moduleIdentities = Identity.NewSource();
			_assemblyLookup = new Dictionary<ICorDebugAssembly, MetaAssembly>();
			_moduleLookup = new Dictionary<ICorDebugModule, ModuleData>();
		}

		public MetaAssembly GetAssembly(ICorDebugAssembly assembly)
		{
			if (!_assemblyLookup.TryGetValue(assembly, out var result))
			{
				_assemblyLookup.Add(assembly, result = CreateAssembly(assembly));
			}

			return result;
		}

		public MetaModule GetModule(ICorDebugModule module)
		{
			return GetModuleData(module).Module;
		}

		public MetaResolvedType GetType(ICorDebugClass cl)
		{
			var module = cl.GetModule();
			var data = GetModuleData(module);
			return GetTypeDef(data, module, cl.GetToken());
		}

		public MetaTypeBase GetType(ICorDebugType type)
		{
			var typeCode = type.GetType();

			switch (typeCode)
			{
				case CorElementType.ELEMENT_TYPE_ARRAY:
				case CorElementType.ELEMENT_TYPE_SZARRAY:
					return new MetaArrayType(GetType(type.GetFirstTypeParameter()), type.GetRank());

				case CorElementType.ELEMENT_TYPE_CLASS:
				case CorElementType.ELEMENT_TYPE_VALUETYPE:
					{
						var baseType = GetType(type.GetClass());
						var typeArgs = type.EnumerateTypeParameters();

						if (typeArgs != null && typeArgs.GetCount() > 0)
						{
							return new MetaGenType(baseType, GetTypes(typeArgs));
						}
						else
						{
							return baseType;
						}
					}

				case CorElementType.ELEMENT_TYPE_PTR:
					return new MetaPointerType(GetType(type.GetFirstTypeParameter()));

				default:
					return GetType(typeCode);
			}
		}

		public MetaTypeBase GetType(ICorDebugModule module, MetaDataToken token)
		{
			var data = GetModuleData(module);

			switch (token.TokenType)
			{
				case TokenType.TypeDef: return GetTypeDef(data, module, token);
				case TokenType.TypeRef: return GetTypeRef(data, module, token);
				case TokenType.TypeSpec: return GetTypeSpec(data, module, token);
				default: throw new ResolutionException();
			}
		}

		public static MetaKnownType GetType(CorElementType type)
		{
			switch (type)
			{
				case CorElementType.ELEMENT_TYPE_BOOLEAN: return MetaKnownType.Boolean;
				case CorElementType.ELEMENT_TYPE_CHAR: return MetaKnownType.Char;
				case CorElementType.ELEMENT_TYPE_I: return MetaKnownType.IntPtr;
				case CorElementType.ELEMENT_TYPE_I1: return MetaKnownType.SByte;
				case CorElementType.ELEMENT_TYPE_I2: return MetaKnownType.Int16;
				case CorElementType.ELEMENT_TYPE_I4: return MetaKnownType.Int32;
				case CorElementType.ELEMENT_TYPE_I8: return MetaKnownType.Int64;
				case CorElementType.ELEMENT_TYPE_OBJECT: return MetaKnownType.Object;
				case CorElementType.ELEMENT_TYPE_R4: return MetaKnownType.Single;
				case CorElementType.ELEMENT_TYPE_R8: return MetaKnownType.Double;
				case CorElementType.ELEMENT_TYPE_STRING: return MetaKnownType.String;
				case CorElementType.ELEMENT_TYPE_TYPEDBYREF: return MetaKnownType.TypedReference;
				case CorElementType.ELEMENT_TYPE_U: return MetaKnownType.UIntPtr;
				case CorElementType.ELEMENT_TYPE_U1: return MetaKnownType.Byte;
				case CorElementType.ELEMENT_TYPE_U2: return MetaKnownType.UInt16;
				case CorElementType.ELEMENT_TYPE_U4: return MetaKnownType.UInt32;
				case CorElementType.ELEMENT_TYPE_U8: return MetaKnownType.UInt64;
				case CorElementType.ELEMENT_TYPE_VOID: return MetaKnownType.Void;
				default: throw new ResolutionException(type);
			}
		}

		public MetaMethod GetMethod(ICorDebugFunction function)
		{
			var module = function.GetModule();
			var data = GetModuleData(module);
			return GetMethod(data, module, function.GetToken());
		}

		public MetaTypeBase[] GetTypes(ICorDebugTypeEnum types)
		{
			var result = new MetaTypeBase[types.GetCount()];

			for (var i = 0; i < result.Length; i++)
			{
				types.Next(1, out var type);
				result[i] = GetType(type);
			}

			return result;
		}

		public MetaField[] GetFields(ICorDebugModule module, MetaDataToken classToken)
		{
			var import = module.GetMetaDataImport();
			var tokens = import.GetFields(classToken);

			if (tokens.Length == 0)
			{
				return Array.Empty<MetaField>();
			}

			var moduleData = GetModuleData(module);
			var result = new MetaField[tokens.Length];

			for (var i = 0; i < tokens.Length; i++)
			{
				result[i] = CreateField(moduleData, module, tokens[i]);
			}

			return result;
		}

		static MetaAssembly CreateAssembly(ICorDebugAssembly assembly)
		{
			var path = assembly.GetName();

			ICorDebugModule module = null;
			var modules = assembly.EnumerateModules();

			while (modules.Next(1, out module))
			{
				if (module.GetMetaDataAssemblyImport().GetAssemblyProps(out var name, out var version, out var locale, out var publicKeyToken))
				{
					return new MetaAssembly(path, name, version, publicKeyToken, locale);
				}
			}

			// No manifested module found, fake it.

			if (string.IsNullOrEmpty(path))
			{
				// not sure how this happens, but it has happened.
				path = "<null-assembly-" + assembly.GetHashCode().ToString("X8", CultureInfo.InvariantCulture) + ">";
			}

			if (module == null)
			{
				path += " - no modules";
			}

			return new MetaAssembly(path, path, new Version(), null, null);
		}

		ModuleData GetModuleData(ICorDebugModule module)
		{
			if (!_moduleLookup.TryGetValue(module, out var data))
			{
				module.GetMetaDataImport().GetScopeProps(out var name, out var mvid);

				var corAssembly = module.GetAssembly();
				var assembly = corAssembly.HasAppDomainInfo() ? GetAssembly(corAssembly) : null;

				var result = new MetaModule(
					assembly,
					_moduleIdentities.New(),
					module.GetName(),
					name,
					module.IsInMemory(),
					module.IsDynamic(),
					mvid);

				if (assembly != null)
				{
					assembly.Modules.Add(result);
				}

				data = new ModuleData(result);

				_moduleLookup.Add(module, data);
			}

			return data;
		}

		MetaResolvedType GetTypeDef(ModuleData data, ICorDebugModule module, MetaDataToken token)
		{
			if (!data.TypeLookup.TryGetValue(token, out var result))
			{
				data.TypeLookup.Add(token, result = CreateTypeDef(data, module, token));
			}

			return (MetaResolvedType)result;
		}

		MetaUnresolvedType GetUnresolvedTypeRef(ModuleData data, ICorDebugModule module, MetaDataToken token)
		{
			if (!data.TypeLookup.TryGetValue(token, out var result))
			{
				data.TypeLookup.Add(token, result = CreateUnresolvedType(data, module, token));
			}

			return (MetaUnresolvedType)result;
		}

		MetaType GetTypeRef(ModuleData data, ICorDebugModule module, MetaDataToken token)
		{
			if (!data.TypeLookup.TryGetValue(token, out var result))
			{
				var defModule = module;
				var defToken = token;

				var resolutionResult = Resolver.TryResolve(ref defModule, ref defToken);

				switch (resolutionResult)
				{
					case ResolutionResult.Success:
						result = GetTypeDef(GetModuleData(defModule), defModule, defToken);
						break;

					case ResolutionResult.NotFound:
						result = GetUnresolvedTypeRef(data, module, token);
						break;

					default:
						throw new ResolutionException(token);
				}
			}

			return result;
		}

		MetaTypeBase GetTypeSpec(ModuleData data, ICorDebugModule module, MetaDataToken token)
		{
			if (!data.TypeSpecLookup.TryGetValue(token, out var result))
			{
				data.TypeSpecLookup.Add(token, result = CreateTypeSpec(data, module, token));
			}

			return result;
		}

		MetaType GetType(ModuleData data, ICorDebugModule module, MetaDataToken token)
		{
			switch (token.TokenType)
			{
				case TokenType.TypeDef: return GetTypeDef(data, module, token);
				case TokenType.TypeRef: return GetTypeRef(data, module, token);
				default: throw new ResolutionException(token);
			}
		}

		MetaResolvedType CreateTypeDef(ModuleData data, ICorDebugModule module, MetaDataToken token)
		{
			var import = module.GetMetaDataImport();
			import.GetTypeDefProps(token, out var name, out var declaringToken, out var baseType);

			MetaAssembly assembly;

			if (declaringToken.IsNil && (assembly = data.Module.Assembly) != null && assembly.IsCorLib)
			{
				var knownType = MetaKnownType.FromFullName(name);

				if (knownType != null)
				{
					return knownType;
				}
				else if (name == MetaNullableType.TypeName)
				{
					return data.Nullable ?? (data.Nullable = CreateNullable(data, import, token));
				}
				else if (name == MetaGCHandleType.TypeName)
				{
					return data.GCHandle ?? (data.GCHandle = CreateGCHandle(data, import, token));
				}
			}

			var declaringType = declaringToken.IsNil ? null : GetTypeDef(data, module, declaringToken);

			if (module.IsEnum(baseType))
			{
				return CreateEnumTypeDef(data, module, token, declaringType, name);
			}
			else
			{
				return CreateRegularTypeDef(data, import, token, declaringType, name);
			}
		}

		MetaUnresolvedType CreateUnresolvedType(ModuleData data, ICorDebugModule module, MetaDataToken token)
		{
			MetaUnresolvedType declaringType;

			var import = module.GetMetaDataImport();
			import.GetTypeRefProps(token, out var scope, out var name);

			if (scope.TokenType == TokenType.TypeRef)
			{
				declaringType = GetUnresolvedTypeRef(data, module, scope);
			}
			else
			{
				declaringType = null;
			}

			return new MetaUnresolvedType(token, declaringType, name);
		}

		MetaTypeBase CreateTypeSpec(ModuleData data, ICorDebugModule module, MetaDataToken token)
		{
			var import = module.GetMetaDataImport();
			import.GetTypeSpecFromToken(token, out var sigPtr, out var sigLen);

			var context = new SigContext(this, module, data);
			return MetaSignatureParser.ReadTypeSig(context, sigPtr, sigLen);
		}

		static MetaResolvedType CreateRegularTypeDef(ModuleData data, IMetaDataImport import, MetaDataToken token, MetaResolvedType declaringType, string name)
		{
			var typeArgCount = import.GetTypeArgCount(token);

			return new MetaSimpleResolvedType(
				data.Module,
				token,
				declaringType,
				name,
				typeArgCount);
		}

		static unsafe MetaEnumType CreateEnumTypeDef(ModuleData data, ICorDebugModule module, MetaDataToken token, MetaResolvedType declaringType, string name)
		{
			var import = module.GetMetaDataImport();
			var labelsList = new List<string>();
			var valuesList = new List<ulong>();
			var hEnum = IntPtr.Zero;
			MetaKnownType underlyingType = null;
			List<IntPtr> ptrList = null;

			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				MetaDataToken fieldToken;

				while (import.EnumFields(ref hEnum, token, &fieldToken, 1) == 1)
				{
					import.GetFieldProps(fieldToken, out var fieldName, out var att, out var valueType, out var valuePtr, out var valueLen);

					if ((att & FieldAttributes.Static) == 0)
					{
						if (fieldName != "value__" || underlyingType != null)
						{
							throw new InvalidMetaDataException("Enums may only contain one instance member and it must be called 'value__'.");
						}

						underlyingType = GetType(valueType);

						if (underlyingType == null || !CanGetEnumValue(underlyingType))
						{
							throw new InvalidMetaDataException("Enum instance member has an invalid type.");
						}

						if (ptrList != null)
						{
							for (var i = 0; i < ptrList.Count; i++)
							{
								valuesList.Add(GetEnumValue(underlyingType, ptrList[i]));
							}

							ptrList = null;
						}
					}
					else if (valuePtr != IntPtr.Zero)
					{
						if (underlyingType == null)
						{
							if (ptrList == null)
							{
								ptrList = new List<IntPtr>();
							}

							ptrList.Add(valuePtr);
						}
						else
						{
							valuesList.Add(GetEnumValue(underlyingType, valuePtr));
						}

						labelsList.Add(fieldName);
					}
				}
			}
			finally
			{
				if (hEnum != IntPtr.Zero)
				{
					import.CloseEnum(hEnum);
					hEnum = IntPtr.Zero;
				}
			}

			if (underlyingType == null)
			{
				throw new InvalidMetaDataException("Enums must contain exactly one instance member.");
			}

			var labels = labelsList.ToArray();
			var values = valuesList.ToArray();
			Array.Sort(values, labels);

			var isFlags = module.HasFlagsAttribute(token);
			return new MetaEnumType(data.Module, token, declaringType, name, underlyingType, isFlags, labels, values);
		}

		static unsafe ulong GetEnumValue(MetaKnownType type, IntPtr valuePtr)
		{
			if (valuePtr == IntPtr.Zero) throw new ArgumentNullException(nameof(valuePtr));

			ulong value;

			switch (type.Size)
			{
				case 1: value = *(byte*)valuePtr; break;
				case 2: value = *(ushort*)valuePtr; break;
				case 4: value = *(uint*)valuePtr; break;
				case 8: value = *(ulong*)valuePtr; break;
				default: throw new InvalidOperationException("Type has no valid size information.");
			}

			return value;
		}

		static bool CanGetEnumValue(MetaKnownType type)
		{
			switch (type.Size)
			{
				case 1:
				case 2:
				case 4:
				case 8:
					return true;
			}

			return false;
		}

		static unsafe MetaNullableType CreateNullable(ModuleData data, IMetaDataImport import, MetaDataToken token)
		{
			var hEnum = IntPtr.Zero;
			var hasValueToken = MetaDataToken.Nil;
			var valueToken = MetaDataToken.Nil;
			MetaDataToken fieldToken;

			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				while (import.EnumFields(ref hEnum, token, &fieldToken, 1) == 1)
				{
					import.GetFieldTypeInfo(fieldToken, out var type, out var _);

					switch (type)
					{
						case CorElementType.ELEMENT_TYPE_VAR:
							if (valueToken != MetaDataToken.Nil)
							{
								throw new InvalidMetaDataException();
							}

							valueToken = fieldToken;
							break;

						case CorElementType.ELEMENT_TYPE_BOOLEAN:
							if (hasValueToken != MetaDataToken.Nil)
							{
								throw new InvalidMetaDataException();
							}

							hasValueToken = fieldToken;
							break;

						default:
							throw new InvalidMetaDataException();
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

			if (hasValueToken.IsNil || valueToken.IsNil)
			{
				throw new InvalidMetaDataException();
			}

			return new MetaNullableType(data.Module, token, hasValueToken, valueToken);
		}

		static unsafe MetaGCHandleType CreateGCHandle(ModuleData data, IMetaDataImport import, MetaDataToken token)
		{
			var hEnum = IntPtr.Zero;
			var handleToken = MetaDataToken.Nil;
			MetaDataToken fieldToken;

			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				while (import.EnumFields(ref hEnum, token, &fieldToken, 1) == 1)
				{
					import.GetFieldTypeInfo(fieldToken, out var type, out var _);

					switch (type)
					{
						case CorElementType.ELEMENT_TYPE_I:
						case CorElementType.ELEMENT_TYPE_U:
							if (handleToken != MetaDataToken.Nil)
							{
								throw new InvalidMetaDataException();
							}

							handleToken = fieldToken;
							break;
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

			if (handleToken.IsNil)
			{
				throw new InvalidMetaDataException();
			}

			return new MetaGCHandleType(data.Module, token, handleToken);
		}

		MetaField CreateField(ModuleData data, ICorDebugModule module, MetaDataToken token)
		{
			var import = module.GetMetaDataImport();
			import.GetFieldProps(token, out var name, out var sigPtr, out var sigLen);

			var context = new SigContext(this, module, data);
			var type = MetaSignatureParser.ReadFieldType(context, sigPtr, sigLen);
			return new MetaField(token, type, name);
		}

		MetaMethod GetMethod(ModuleData data, ICorDebugModule module, MetaDataToken token)
		{
			if (!data.MethodLookup.TryGetValue(token, out var result))
			{
				var import = module.GetMetaDataImport();
				import.GetMethodProps(token, out var declaringToken, out var name, out var sigPtr, out var sigLen, out var rva);

				MetaModule declaringModule;
				MetaResolvedType declaringType;

				if (declaringToken.IsNil)
				{
					declaringType = null;
					declaringModule = GetModule(module);
				}
				else
				{
					declaringType = GetTypeDef(data, module, declaringToken);
					declaringModule = declaringType.Module ?? GetModule(module);
				}

				var context = new MethodSigContext(this, module, data, token);
				var sig = MetaSignatureParser.ReadMethodDefSig(context, sigPtr, sigLen);
				var localToken = GetLocalSigToken(module, rva);

				MetaVariable[] localSig;

				if (localToken.IsNil)
				{
					localSig = null;
				}
				else
				{
					import.GetSigFromToken(localToken, out sigPtr, out sigLen);
					localSig = MetaSignatureParser.ReadLocalSig(context, sigPtr, sigLen);
				}

				result = new MetaMethod(
					token,
					declaringModule,
					declaringType,
					name,
					sig,
					localSig);

				data.MethodLookup.Add(token, result);
			}

			return result;
		}

		static MetaDataToken GetLocalSigToken(ICorDebugModule module, int rva)
		{
			if (rva == 0)
			{
				return MetaDataToken.Nil;
			}

			var baseAddress = module.GetBaseAddress();

			if (baseAddress.IsNull)
			{
				return MetaDataToken.Nil;
			}

			var methodAddress = baseAddress + rva;
			var process = module.GetProcess();

			if ((process.ReadInt(methodAddress) & 0x03) != 0x03)
			{
				// if it's not using the fat header, then there can't be any locals.
				return MetaDataToken.Nil;
			}

			return new MetaDataToken(process.ReadInt(methodAddress + 8));
		}

		readonly IIdentitySource _moduleIdentities;
		readonly Dictionary<ICorDebugAssembly, MetaAssembly> _assemblyLookup;
		readonly Dictionary<ICorDebugModule, ModuleData> _moduleLookup;

		sealed class ModuleData
		{
			public ModuleData(MetaModule module)
			{
				Module = module;
				TypeLookup = new Dictionary<MetaDataToken, MetaType>();
				TypeSpecLookup = new Dictionary<MetaDataToken, MetaTypeBase>();
				MethodLookup = new Dictionary<MetaDataToken, MetaMethod>();
			}

			public readonly MetaModule Module;
			public readonly Dictionary<MetaDataToken, MetaType> TypeLookup;
			public readonly Dictionary<MetaDataToken, MetaTypeBase> TypeSpecLookup;
			public readonly Dictionary<MetaDataToken, MetaMethod> MethodLookup;
			public MetaNullableType Nullable;
			public MetaGCHandleType GCHandle;
		}

		class SigContext : ISignatureContext
		{
			public SigContext(MetaDataCache cache, ICorDebugModule module, ModuleData data)
			{
				_cache = cache;
				Module = module;
				_data = data;
			}

			public MetaType GetType(MetaDataToken token) => _cache.GetType(_data, Module, token);
			public virtual string GetParamName(int index) => throw new NotSupportedException();
			public virtual string GetLocalName(int index) => throw new NotSupportedException();
			protected ICorDebugModule Module { get; }

			readonly MetaDataCache _cache;
			readonly ModuleData _data;
		}

		sealed class MethodSigContext : SigContext
		{
			public MethodSigContext(MetaDataCache cache, ICorDebugModule module, ModuleData data, MetaDataToken methodToken)
				: base(cache, module, data)
			{
				_methodToken = methodToken;
			}

			public override string GetParamName(int index) => Module.GetMetaDataImport().GetParamName(_methodToken, index);
			public override string GetLocalName(int index) => "$local" + index;
			readonly MetaDataToken _methodToken;
		}
	}
}
