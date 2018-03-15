// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Runtime.InteropServices;
using WAYWF.Agent.CorDebugApi;

namespace WAYWF.Agent
{
	static class FrameExtensions
	{
		public static ICorDebugValue GetArgument(this ICorDebugILFrame frame, int index)
		{
			var hr = frame.GetArgument(index, out var value);

			if (hr == HResults.CORDBG_E_IL_VAR_NOT_AVAILABLE ||
				hr == HResults.CORDBG_E_CLASS_NOT_LOADED ||
				hr == HResults.CORDBG_E_READVIRTUAL_FAILURE)
			{
				return null;
			}
			else if (hr < 0)
			{
				throw Marshal.GetExceptionForHR(hr);
			}
			else
			{
				return value;
			}
		}

		public static ICorDebugValue GetLocalVariable(this ICorDebugILFrame frame, int index)
		{
			var hr = frame.GetLocalVariable(index, out var value);

			if (hr == HResults.CORDBG_E_IL_VAR_NOT_AVAILABLE ||
				hr == HResults.CORDBG_E_CLASS_NOT_LOADED ||
				hr == HResults.CORDBG_E_READVIRTUAL_FAILURE)
			{
				return null;
			}
			else if (hr < 0)
			{
				throw Marshal.GetExceptionForHR(hr);
			}
			else
			{
				return value;
			}
		}
	}
}
