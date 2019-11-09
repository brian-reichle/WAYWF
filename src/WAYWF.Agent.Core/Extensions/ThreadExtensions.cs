// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Runtime.InteropServices;
using WAYWF.Agent.Core.CorDebugApi;

namespace WAYWF.Agent.Core
{
	static class ThreadExtensions
	{
		public static ICorDebugChainEnum EnumerateChains(this ICorDebugThread thread)
		{
			var hr = thread.EnumerateChains(out var result);

			switch (hr)
			{
				case HResults.CORDBG_E_BAD_THREAD_STATE:
				case HResults.CORDBG_E_THREAD_NOT_SCHEDULED:
					return null;
			}

			if (hr < 0)
			{
				throw Marshal.GetExceptionForHR(hr);
			}

			return result;
		}
	}
}
