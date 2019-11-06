// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using WAYWF.Agent.CorDebugApi;
using WAYWF.Agent.Data;

namespace WAYWF.Agent
{
	static class ValueExtensions
	{
		public static ICorDebugValue Dereference(this ICorDebugReferenceValue reference)
		{
			var hr = reference.Dereference(out var result);

			if (hr >= 0)
			{
				return result;
			}

			switch (hr)
			{
				case HResults.CORDBG_E_CLASS_NOT_LOADED: // WTF??
				case HResults.CORDBG_E_FIELD_NOT_AVAILABLE:
				case HResults.CORDBG_E_BAD_REFERENCE_VALUE: // can happen when trying to dereference an invalid pointer. eg. (byte*)1
				case HResults.CORDBG_E_READVIRTUAL_FAILURE:
					return null;

				default:
					throw Marshal.GetExceptionForHR(hr);
			}
		}

		public static unsafe string GetString(this ICorDebugStringValue value)
		{
			var length = value.GetLength();

			if (length == 0)
			{
				return string.Empty;
			}
			else if (length < 512)
			{
				var buffer = stackalloc char[length];
				value.GetString(length, out length, buffer);
				return new string(buffer, 0, length);
			}
			else
			{
				var buffer = new char[length];

				fixed (char* ptr = buffer)
				{
					value.GetString(length, out length, ptr);
				}

				return new string(buffer, 0, length);
			}
		}

		public static ICorDebugValue GetFieldValue(this ICorDebugObjectValue value, MetaDataToken token)
		{
			var hr = value.GetFieldValue(value.GetClass(), token, out var result);

			if (hr < 0)
			{
				switch (hr)
				{
					case HResults.E_FAIL: // WTF??
					case HResults.CORDBG_E_FIELD_NOT_AVAILABLE:
					case HResults.CORDBG_E_READVIRTUAL_FAILURE:
						return null;

					default:
						throw Marshal.GetExceptionForHR(hr);
				}
			}

			return result;
		}

		public static CORDB_ADDRESS[] GetInterfacePointers(this ICorDebugComObjectValue value)
		{
			value.GetCachedInterfacePointers(false, 0, out var size, null);

			var result = new CORDB_ADDRESS[size];
			value.GetCachedInterfacePointers(false, size, out size, result);

			return result;
		}

		public static unsafe T GetValue<T>(this ICorDebugGenericValue value)
			where T : unmanaged
		{
			if (value.GetSize() != sizeof(T)) throw new ArgumentNullException(nameof(value), "size mismatch");

			T tmp;
			value.GetValue((IntPtr)(&tmp));
			return tmp;
		}
	}
}
