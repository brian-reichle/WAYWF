// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("CC7BCAEC-8A68-11D2-983C-0000F808342D")]
	interface ICorDebugStepper
	{
		// HRESULT IsActive(
		//     [out] BOOL   *pbActive
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		bool IsActive();

		// HRESULT Deactivate();
		void Deactivate();

		// HRESULT SetInterceptMask(
		//     [in] CorDebugIntercept    mask
		// );
		void SetInterceptMask_();

		// HRESULT SetUnmappedStopMask(
		//     [in] CorDebugUnmappedStop   mask
		// );
		void SetUnmappedStopMask_();

		// HRESULT Step(
		//     [in] BOOL   bStepIn
		// );
		void Step(
			bool bStepIn);

		// HRESULT StepRange(
		//     [in] BOOL     bStepIn,
		//     [in, size_is(cRangeCount)] COR_DEBUG_STEP_RANGE ranges[],
		//     [in] ULONG32  cRangeCount
		// );
		void StepRange_();

		// HRESULT StepOut();
		void StepOut();

		// HRESULT SetRangeIL(
		//     [in] BOOL    bIL
		// );
		void SetRangeIL(
			[MarshalAs(UnmanagedType.Bool)] bool bIL);
	}
}
