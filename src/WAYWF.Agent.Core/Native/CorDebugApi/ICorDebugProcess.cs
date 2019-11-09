// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.Core.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("3D6F5F64-7538-11D3-8D5B-00104B35E7EF")]
	interface ICorDebugProcess : ICorDebugController
	{
		// HRESULT Stop(
		//     [in] DWORD dwTimeoutIgnored
		// );
		new void Stop(
			int pwTimeoutIgnored = 0);

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
			[MarshalAs(UnmanagedType.Interface)] ICorDebugThread pThread = null);

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

		// HRESULT GetID(
		//     [out] DWORD *pdwProcessId
		// );
		int GetID();

		// HRESULT GetHandle(
		//     [out] HPROCESS *phProcessHandle
		// );
		void GetHandle_();

		// HRESULT GetThread(
		//     [in] DWORD dwThreadId,
		//     [out] ICorDebugThread **ppThread
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugThread GetThread(
			int dwThreadId);

		// HRESULT EnumerateObjects(
		//     [out] ICorDebugObjectEnum **ppObjects
		// );
		void EnumerateObjects_();

		// HRESULT IsTransitionStub(
		//     [in]  CORDB_ADDRESS address,
		//     [out] BOOL *pbTransitionStub
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		bool IsTransitionStub(
			CORDB_ADDRESS address);

		// HRESULT IsOSSuspended(
		//     [in]  DWORD threadID,
		//     [out] BOOL  *pbSuspended
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		bool IsOSSuspended(
			int threadID);

		// HRESULT GetThreadContext(
		//     [in] DWORD threadID,
		//     [in] ULONG32 contextSize,
		//     [in, out, length_is(contextSize), size_is(contextSize)] BYTE context[]
		// );
		void GetThreadContext_();

		// HRESULT SetThreadContext(
		//     [in] DWORD threadID,
		//     [in] ULONG32 contextSize,
		//     [in, length_is(contextSize), size_is(contextSize)] BYTE context[]
		// );
		void SetThreadContext_();

		// HRESULT ReadMemory(
		//     [in]  CORDB_ADDRESS address,
		//     [in]  DWORD size,
		//     [out, size_is(size), length_is(size)] BYTE buffer[],
		//     [out] SIZE_T *read
		// );
		IntPtr ReadMemory(
			CORDB_ADDRESS address,
			int size,
			IntPtr buffer);

		// HRESULT WriteMemory(
		//     [in]  CORDB_ADDRESS address,
		//     [in]  DWORD size,
		//     [in, size_is(size)] BYTE buffer[],
		//     [out] SIZE_T *written
		// );
		IntPtr WriteMemory(
			CORDB_ADDRESS address,
			int size,
			IntPtr buffer);

		// HRESULT ClearCurrentException(
		//     [in] DWORD threadID
		// );
		void ClearCurrentException(
			int threadID);

		// HRESULT EnableLogMessages(
		//     [in]BOOL fOnOff
		// );
		void EnableLogMessages(
			[MarshalAs(UnmanagedType.Bool)] bool fOnOff);

		// HRESULT ModifyLogSwitch(
		//     [in] WCHAR *pLogSwitchName,
		//     [in] LONG  lLevel
		// );
		void ModifyLogSwitch(
			[MarshalAs(UnmanagedType.LPWStr)] string pLogSwitchName,
			int lLevel);

		// HRESULT EnumerateAppDomains(
		//     [out] ICorDebugAppDomainEnum **ppAppDomains
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugAppDomainEnum EnumerateAppDomains();

		// HRESULT GetObject(
		//     [out] ICorDebugValue **ppObject
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugValue GetObject();

		// HRESULT ThreadForFiberCookie(
		//     [in] DWORD fiberCookie,
		//     [out] ICorDebugThread **ppThread
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		ICorDebugThread ThreadForFiberCookie(
			int fiberCookie);

		// HRESULT GetHelperThreadID(
		//     [out] DWORD *pThreadID
		// );
		int GetHelperThreadID();
	}
}
