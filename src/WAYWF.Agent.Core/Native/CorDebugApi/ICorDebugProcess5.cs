// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WAYWF.Agent.Core.CorDebugApi
{
	[ComImport]
	[SuppressUnmanagedCodeSecurity]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("21E9D9C0-FCB8-11DF-8CFF-0800200C9A66")]
	interface ICorDebugProcess5
	{
		// HRESULT GetGCHeapInformation(
		//     [out] COR_HEAPINFO *pHeapInfo
		// );
		void GetGCHeapInformation(
			out COR_HEAPINFO pHeapInfo);

		// HRESULT EnumerateHeap(
		//     [out] ICorDebugHeapEnum **ppObjects
		// );
		ICorDebugHeapEnum EnumerateHeap();

		// HRESULT EnumerateHeapRegions(
		//     [out] ICorDebugHeapSegmentEnum **ppRegions
		// );
		void EnumerateHeapRegions_();

		// HRESULT GetObject(
		//     [in] CORDB_ADDRESS addr,
		//     [out] ICorDebugObjectValue **ppObject
		// );
		ICorDebugObjectValue GetObject(CORDB_ADDRESS addr);

		// HRESULT EnumerateGCReferences(
		//     [in] Bool enumerateWeakReferences,
		//     [out] ICorDebugGCReferenceEnum **ppEnum
		// );
		void EnumerateGCReferences_();

		// HRESULT EnumerateHandles(
		//     [in] CorGCReferenceType types,
		//     [out] ICorDebugGCReferenceEnum **ppEnum
		// );
		void EnumerateHandles_();

		// HRESULT GetTypeID(
		//     [in] CORDB_ADDRESS obj,
		//     [out] COR_TYPEID *pId
		// );
		void GetTypeID_();

		// HRESULT GetTypeForTypeID(
		//     [in] COR_TYPEID id,
		//     [out] ICorDebugType **ppType
		// );
		ICorDebugType GetTypeForTypeID(
			COR_TYPEID id);

		// HRESULT GetArrayLayout(
		//     [in] COR_TYPEID id,
		//     [out] COR_ARRAY_LAYOUT *pLayout
		// );
		void GetArrayLayout_();

		// HRESULT GetTypeLayout(
		//     [in] COR_TYPEID id,
		//     [out] COR_TYPE_LAYOUT *pLayout
		// );
		void GetTypeLayout_();

		// HRESULT GetTypeFields(
		//     [in] COR_TYPEID id,
		//     [in] ULONG32 celt,
		//     [out] COR_FIELD fields[],
		//     [out] ULONG32 *pceltNeeded
		// );
		void GetTypeFields_();

		// HRESULT EnableNGENPolicy(
		//     [in] CorDebugNGENPolicy ePolicy
		// );
		void EnableNGENPolicy_();
	}
}
