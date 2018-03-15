// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using WAYWF.Agent.SymbolStoreApi;

namespace WAYWF.Agent
{
	static class ISymMethodExtensions
	{
		public static string[] GetVariableNames(this ISymUnmanagedMethod method, int ilOffset, int localCount)
		{
			var scope = method.GetRootScope();

			if (scope == null || localCount == 0)
			{
				return null;
			}

			var names = new string[localCount];
			GetVariableNames(scope, ilOffset, names);
			return names;
		}

		static void GetVariableNames(ISymUnmanagedScope scope, int ilOffset, string[] names)
		{
			var children = GetChildren(scope);

			if (children != null)
			{
				foreach (var child in children)
				{
					GetVariableNames(child, ilOffset, names);
				}
			}

			CopyLocals(scope, names);
		}

		static void CopyLocals(ISymUnmanagedScope scope, string[] names)
		{
			var locals = GetLocals(scope);

			if (locals != null)
			{
				foreach (var local in locals)
				{
					if (local.GetAddressKind() == CorSymAddrKind.ADDR_IL_OFFSET)
					{
						var index = local.GetAddressField1();

						if (index < names.Length && names[index] == null)
						{
							names[index] = local.GetName();
						}
					}
				}
			}
		}

		static ISymUnmanagedScope[] GetChildren(ISymUnmanagedScope scope)
		{
			scope.GetChildren(0, out var size, null);

			if (size == 0)
			{
				return null;
			}

			var children = new ISymUnmanagedScope[size];
			scope.GetChildren(children.Length, out size, children);

			return children;
		}

		static ISymUnmanagedVariable[] GetLocals(ISymUnmanagedScope scope)
		{
			var count = scope.GetLocalCount();

			if (count == 0)
			{
				return null;
			}

			var result = new ISymUnmanagedVariable[count];
			scope.GetLocals(count, out count, result);
			return result;
		}

		static unsafe string GetName(this ISymUnmanagedVariable variable)
		{
			variable.GetName(0, out var size, null);

			if (size <= 1)
			{
				return string.Empty;
			}

			var buffer = stackalloc char[size];
			variable.GetName(size, out size, buffer);
			return new string(buffer, 0, size - 1);
		}
	}
}
