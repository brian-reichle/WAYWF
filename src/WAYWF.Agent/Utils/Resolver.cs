// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Runtime.InteropServices;
using WAYWF.Agent.CorDebugApi;

namespace WAYWF.Agent
{
	static class Resolver
	{
		public static ResolutionResult TryResolve(ref ICorDebugModule module, ref MetaDataToken token)
		{
			var refImport = module.GetMetaDataImport();
			refImport.GetTypeRefProps(token, out var scope, out var className);

			if (scope.TokenType == TokenType.TypeRef)
			{
				var result = TryResolve(ref module, ref scope);

				if (result != ResolutionResult.Success)
				{
					return result;
				}

				var defImport = module.GetMetaDataImport();
				var hr = defImport.FindTypeDefByName(className, scope, out token);

				if (hr == HResults.CLDB_E_RECORD_NOTFOUND)
				{
					module = null;
					return ResolutionResult.Fail;
				}
				else if (hr < 0)
				{
					throw Marshal.GetExceptionForHR(hr);
				}
				else if (token.IsNil)
				{
					module = null;
					return ResolutionResult.Fail;
				}

				return ResolutionResult.Success;
			}
			else if (scope == MetaDataToken.Nil)
			{
				return TryResolveExportedType(ref module, className, out token);
			}
			else if (scope == MetaDataToken.Module)
			{
				return TryResolveTypeByName(module, className, out token);
			}
			else
			{
				var assembly = module.ResolveAssembly(scope);

				if (assembly == null)
				{
					return ResolutionResult.NotFound;
				}

				return TryResolveTypeByName(assembly, className, out module, out token);
			}
		}

		static ResolutionResult TryResolveTypeByName(ICorDebugAssembly assembly, string className, out ICorDebugModule module, out MetaDataToken token)
		{
			var moduleEnum = assembly.EnumerateModules();

			ICorDebugModule fallbackModule = null;
			ResolutionResult result;

			while (moduleEnum.Next(1, out var otherModule))
			{
				if (fallbackModule == null)
				{
					fallbackModule = otherModule;
				}

				result = TryResolveTypeByName(otherModule, className, out token);

				if (result == ResolutionResult.Success)
				{
					module = otherModule;
					return ResolutionResult.Success;
				}
				else if (result == ResolutionResult.Fail)
				{
					module = null;
					return ResolutionResult.Fail;
				}
			}

			if (fallbackModule == null)
			{
				throw new ShitFanContactException("no module?");
			}

			result = TryResolveExportedType(ref fallbackModule, className, out token);

			if (result == ResolutionResult.Success)
			{
				module = fallbackModule;
				return ResolutionResult.Success;
			}
			else
			{
				module = null;
				token = MetaDataToken.Nil;
				return result;
			}
		}

		static ResolutionResult TryResolveTypeByName(ICorDebugModule module, string className, out MetaDataToken token)
		{
			var importRef = module.GetMetaDataImport();
			var hr = importRef.FindTypeDefByName(className, MetaDataToken.Nil, out token);

			if (hr == HResults.CLDB_E_RECORD_NOTFOUND)
			{
				return ResolutionResult.NotFound;
			}
			else if (hr < 0)
			{
				throw Marshal.GetExceptionForHR(hr);
			}
			else if (token.IsNil)
			{
				return ResolutionResult.NotFound;
			}
			else
			{
				return ResolutionResult.Success;
			}
		}

		static ResolutionResult TryResolveExportedType(ref ICorDebugModule module, string className, out MetaDataToken token)
		{
			var aImport = module.GetMetaDataAssemblyImport();

			var hr = aImport.FindExportedTypeByName(className, MetaDataToken.Nil, out var mdExportedType);

			if (hr == HResults.CLDB_E_RECORD_NOTFOUND)
			{
				module = null;
				token = MetaDataToken.Nil;
				return ResolutionResult.NotFound;
			}
			else if (hr < 0)
			{
				throw Marshal.GetExceptionForHR(hr);
			}

			var implementation = aImport.GetExportedTypeImplementation(mdExportedType);

			if (implementation.TokenType == TokenType.AssemblyRef)
			{
				var assembly = module.ResolveAssembly(implementation);

				if (assembly == null)
				{
					token = MetaDataToken.Nil;
					return ResolutionResult.NotFound;
				}

				return TryResolveTypeByName(assembly, className, out module, out token);
			}

			module = null;
			token = MetaDataToken.Nil;
			return ResolutionResult.NotFound;
		}
	}
}
