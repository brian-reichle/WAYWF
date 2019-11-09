// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.Core.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("3D6F5F63-7538-11D3-8D5B-00104B35E7EF")]
	unsafe interface ICorDebugAppDomain : ICorDebugController
	{
		// HRESULT Stop(
		//     [in] DWORD dwTimeoutIgnored
		// );
		new void Stop(
			int dwTimeoutIgnored = 0);

		// HRESULT Continue(
		//     [in] BOOL fIsOutOfBand
		// );
		new void Continue(
			[MarshalAs(UnmanagedType.Bool)] bool fIsOutOfBand = false);

		// HRESULT IsRunning(
		//     [out] BOOL *pbRunning
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		new bool IsRunning();

		// HRESULT HasQueuedCallbacks(
		//     [in] ICorDebugThread *pThread,
		//     [out] BOOL *pbQueued
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		new bool HasQueuedCallbacks(
			[MarshalAs(UnmanagedType.Interface)] ICorDebugThread pThread);

		// HRESULT EnumerateThreads(
		//     [out] ICorDebugThreadEnum **ppThreads
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		new ICorDebugThreadEnum EnumerateThreads();

		// HRESULT SetAllThreadsDebugState(
		//     [in] CorDebugThreadState state,
		//     [in] ICorDebugThread *pExceptThisThread
		// );
		new void SetAllThreadsDebugState(
			CorDebugThreadState state,
			[MarshalAs(UnmanagedType.Interface)] ICorDebugThread pExceptThisThread);

		// HRESULT Detach();
		new void Detach();

		// HRESULT Terminate(
		//     [in] UINT exitCode
		// );
		new void Terminate(
			int exitCode = 0);

		// /*  OBSOLETE  */
		// HRESULT CanCommitChanges(
		//     [in] ULONG cSnapshots,
		//     [in, size_is(cSnapshots)] ICorDebugEditAndContinueSnapshot *pSnapshots[],
		//     [out] ICorDebugErrorInfoEnum **pError
		// );
		new void CanCommitChanges_();

		// /*  OBSOLETE  */
		// HRESULT CommitChanges(
		//     [in] ULONG cSnapshots,
		//     [in, size_is(cSnapshots)] ICorDebugEditAndContinueSnapshot *pSnapshots[],
		//     [out] ICorDebugErrorInfoEnum **pError
		// );
		new void CommitChanges_();

		// HRESULT GetProcess(
		//     [out] ICorDebugProcess   **ppProcess
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugProcess GetProcess();

		// HRESULT EnumerateAssemblies(
		//     [out] ICorDebugAssemblyEnum  **ppAssemblies
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugAssemblyEnum EnumerateAssemblies();

		// HRESULT GetModuleFromMetaDataInterface(
		//     [in] IUnknown           *pIMetaData,
		//     [out] ICorDebugModule  **ppModule
		// );
		[PreserveSig]
		int GetModuleFromMetaDataInterface(
			[MarshalAs(UnmanagedType.Interface)] object pIMetaData,
			[MarshalAs(UnmanagedType.Interface)] out ICorDebugModule ppModule);

		// HRESULT EnumerateBreakpoints(
		//     [out] ICorDebugBreakpointEnum   **ppBreakpoints
		// );
		void EnumerateBreakpoints_();

		// HRESULT EnumerateSteppers(
		//     [out] ICorDebugStepperEnum   **ppSteppers
		// );
		void EnumerateSteppers_();

		// HRESULT IsAttached(
		//     [out] BOOL  *pbAttached
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		bool IsAttached();

		// HRESULT GetName(
		//     [in]  ULONG32           cchName,
		//     [out] ULONG32           *pcchName,
		//     [out, size_is(cchName), length_is(*pcchName)] WCHAR szName[]
		// );
		void GetName(
			int cchName,
			out int pcchName,
			char* szName);

		// HRESULT GetObject(
		//     [out] ICorDebugValue   **ppObject
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugValue GetObject();

		// HRESULT Attach();
		void Attach();

		// HRESULT GetID(
		//     [out] ULONG32   *pId
		// );
		int GetID();
	}
}
