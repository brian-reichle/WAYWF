// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;

namespace WAYWF.Agent.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("3D6F5F60-7538-11D3-8D5B-00104B35E7EF")]
	interface ICorDebugManagedCallback
	{
		// HRESULT Breakpoint(
		//     [in] ICorDebugAppDomain  *pAppDomain,
		//     [in] ICorDebugThread     *pThread,
		//     [in] ICorDebugBreakpoint *pBreakpoint
		// );
		void Breakpoint(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugAppDomain pAppDomain,
			[MarshalAs(UnmanagedType.Interface)] ICorDebugThread pThread,
			IntPtr pBreakpoint);

		// HRESULT StepComplete(
		//     [in] ICorDebugAppDomain  *pAppDomain,
		//     [in] ICorDebugThread     *pThread,
		//     [in] ICorDebugStepper    *pStepper,
		//     [in] CorDebugStepReason   reason
		// );
		void StepComplete(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugAppDomain pAppDomain,
			[MarshalAs(UnmanagedType.Interface)] ICorDebugThread pThread,
			[MarshalAs(UnmanagedType.Interface)] ICorDebugStepper pStepper,
			int reason);

		// HRESULT Break(
		//     [in] ICorDebugAppDomain *pAppDomain,
		//     [in] ICorDebugThread    *thread
		// );
		void Break(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugAppDomain pAppDomain,
			[MarshalAs(UnmanagedType.Interface)] ICorDebugThread thread);

		// HRESULT Exception(
		//     [in] ICorDebugAppDomain *pAppDomain,
		//     [in] ICorDebugThread    *pThread,
		//     [in] BOOL                unhandled
		// );
		void Exception(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugAppDomain pAppDomain,
			[MarshalAs(UnmanagedType.Interface)] ICorDebugThread pThread,
			[MarshalAs(UnmanagedType.Bool)] bool unhandled);

		// HRESULT EvalComplete(
		//     [in] ICorDebugAppDomain *pAppDomain,
		//     [in] ICorDebugThread    *pThread,
		//     [in] ICorDebugEval      *pEval
		// );
		void EvalComplete(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugAppDomain pAppDomain,
			[MarshalAs(UnmanagedType.Interface)] ICorDebugThread pThread,
			IntPtr pEval);

		// HRESULT EvalException(
		//     [in] ICorDebugAppDomain *pAppDomain,
		//     [in] ICorDebugThread    *pThread,
		//     [in] ICorDebugEval      *pEval
		// );
		void EvalException(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugAppDomain pAppDomain,
			[MarshalAs(UnmanagedType.Interface)] ICorDebugThread pThread,
			IntPtr pEval);

		// HRESULT CreateProcess(
		//     [in] ICorDebugProcess *pProcess
		// );
		void CreateProcess(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugProcess pProcess);

		// HRESULT ExitProcess(
		//     [in] ICorDebugProcess *pProcess
		// );
		void ExitProcess(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugProcess pProcess);

		// HRESULT CreateThread(
		//     [in] ICorDebugAppDomain *pAppDomain,
		//     [in] ICorDebugThread    *thread
		// );
		void CreateThread(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugAppDomain pAppDomain,
			[MarshalAs(UnmanagedType.Interface)] ICorDebugThread thread);

		// HRESULT ExitThread(
		//     [in] ICorDebugAppDomain *pAppDomain,
		//     [in] ICorDebugThread    *thread
		// );
		void ExitThread(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugAppDomain pAppDomain,
			[MarshalAs(UnmanagedType.Interface)] ICorDebugThread thread);

		// HRESULT LoadModule(
		//     [in] ICorDebugAppDomain *pAppDomain,
		//     [in] ICorDebugModule    *pModule
		// );
		void LoadModule(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugAppDomain pAppDomain,
			[MarshalAs(UnmanagedType.Interface)] ICorDebugModule pModule);

		// HRESULT UnloadModule(
		//     [in] ICorDebugAppDomain  *pAppDomain,
		//     [in] ICorDebugModule     *pModule
		// );
		void UnloadModule(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugAppDomain pAppDomain,
			[MarshalAs(UnmanagedType.Interface)] ICorDebugModule pModule);

		// HRESULT LoadClass(
		//     [in] ICorDebugAppDomain *pAppDomain,
		//     [in] ICorDebugClass     *c
		// );
		void LoadClass(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugAppDomain pAppDomain,
			[MarshalAs(UnmanagedType.Interface)] IntPtr c);

		// HRESULT UnloadClass(
		//     [in] ICorDebugAppDomain  *pAppDomain,
		//     [in] ICorDebugClass      *c
		// );
		void UnloadClass(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugAppDomain pAppDomain,
			[MarshalAs(UnmanagedType.Interface)] IntPtr c);

		// HRESULT DebuggerError(
		//     [in] ICorDebugProcess *pProcess,
		//     [in] HRESULT           errorHR,
		//     [in] DWORD             errorCode
		// );
		void DebuggerError(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugProcess pProcess,
			int errorHR,
			int errorCode);

		// HRESULT LogMessage(
		//     [in] ICorDebugAppDomain  *pAppDomain,
		//     [in] ICorDebugThread     *pThread,
		//     [in] LONG                 lLevel,
		//     [in] WCHAR               *pLogSwitchName,
		//     [in] WCHAR               *pMessage
		// );
		void LogMessage(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugAppDomain pAppDomain,
			[MarshalAs(UnmanagedType.Interface)] ICorDebugThread pThread,
			int lLevel,
			[MarshalAs(UnmanagedType.LPWStr)] string pLogSwitchName,
			[MarshalAs(UnmanagedType.LPWStr)] string pMessage);

		// HRESULT LogSwitch(
		//     [in] ICorDebugAppDomain  *pAppDomain,
		//     [in] ICorDebugThread     *pThread,
		//     [in] LONG                 lLevel,
		//     [in] ULONG                ulReason,
		//     [in] WCHAR               *pLogSwitchName,
		//     [in] WCHAR               *pParentName
		// );
		void LogSwitch(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugAppDomain pAppDomain,
			[MarshalAs(UnmanagedType.Interface)] ICorDebugThread pThread,
			int lLevel,
			int ulReason,
			[MarshalAs(UnmanagedType.LPWStr)] string pLogSwitchName,
			[MarshalAs(UnmanagedType.LPWStr)] string pParentName);

		// HRESULT CreateAppDomain(
		//     [in] ICorDebugProcess   *pProcess,
		//     [in] ICorDebugAppDomain *pAppDomain
		// );
		void CreateAppDomain(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugProcess pProcess,
			[MarshalAs(UnmanagedType.Interface)] ICorDebugAppDomain pAppDomain);

		// HRESULT ExitAppDomain(
		//     [in] ICorDebugProcess   *pProcess,
		//     [in] ICorDebugAppDomain *pAppDomain
		// );
		void ExitAppDomain(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugProcess pProcess,
			[MarshalAs(UnmanagedType.Interface)] ICorDebugAppDomain pAppDomain);

		// HRESULT LoadAssembly(
		//     [in] ICorDebugAppDomain *pAppDomain,
		//     [in] ICorDebugAssembly  *pAssembly
		// );
		void LoadAssembly(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugAppDomain pAppDomain,
			[MarshalAs(UnmanagedType.Interface)] ICorDebugAssembly pAssembly);

		// HRESULT UnloadAssembly(
		//     [in] IcorDebugAppDomain  *pAppDomain,
		//     [in] ICorDebugAssembly   *pAssembly
		// );
		void UnloadAssembly(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugAppDomain pAppDomain,
			[MarshalAs(UnmanagedType.Interface)] ICorDebugAssembly pAssembly);

		// HRESULT ControlCTrap(
		//     [in] ICorDebugProcess *pProcess
		// );
		void ControlCTrap(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugProcess pProcess);

		// HRESULT NameChange(
		//     [in] ICorDebugAppDomain *pAppDomain,
		//     [in] ICorDebugThread    *pThread
		// );
		void NameChange(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugAppDomain pAppDomain,
			[MarshalAs(UnmanagedType.Interface)] ICorDebugThread pThread);

		// HRESULT UpdateModuleSymbols(
		//     [in] ICorDebugAppDomain *pAppDomain,
		//     [in] ICorDebugModule    *pModule,
		//     [in] IStream            *pSymbolStream
		// );
		void UpdateModuleSymbols(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugAppDomain pAppDomain,
			[MarshalAs(UnmanagedType.Interface)] ICorDebugModule pMmodule,
			[MarshalAs(UnmanagedType.Interface)] IStream pSymbolStream);

		// HRESULT EditAndContinueRemap(
		//     [in] ICorDebugAppDomain *pAppDomain,
		//     [in] ICorDebugThread    *pThread,
		//     [in] ICorDebugFunction  *pFunction,
		//     [in] BOOL fAccurate
		// );
		void EditAndContinueRemap(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugAppDomain pAppDomain,
			[MarshalAs(UnmanagedType.Interface)] ICorDebugThread pThread,
			[MarshalAs(UnmanagedType.Interface)] IntPtr pFunction,
			[MarshalAs(UnmanagedType.Bool)] bool fAccurate);

		// HRESULT BreakpointSetError(
		//     [in] ICorDebugAppDomain  *pAppDomain,
		//     [in] ICorDebugThread     *pThread,
		//     [in] ICorDebugBreakpoint *pBreakpoint,
		//     [in] DWORD                dwError
		// );
		void BreakpointSetError(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugAppDomain pAppDomain,
			[MarshalAs(UnmanagedType.Interface)] ICorDebugThread pThread,
			[MarshalAs(UnmanagedType.Interface)] IntPtr pBreakpoint,
			int dwError);
	}
}
