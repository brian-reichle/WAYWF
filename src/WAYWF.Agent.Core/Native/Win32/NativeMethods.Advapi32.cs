// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace WAYWF.Agent.Core.Win32
{
	partial class NativeMethods
	{
		// BOOL CryptAcquireContext(
		//     [out]  HCRYPTPROV *phProv,
		//     [in]   LPCTSTR     pszContainer,
		//     [in]   LPCTSTR     pszProvider,
		//     [in]   DWORD       dwProvType,
		//     [in]   DWORD       dwFlags
		// );
		[DllImport("advapi32.dll", EntryPoint = "CryptAcquireContextW", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CryptAcquireContext(
			out CryptContextHandle phProv,
			[MarshalAs(UnmanagedType.LPWStr)] string pszContainer,
			[MarshalAs(UnmanagedType.LPWStr)] string pszProvider,
			int dwProvType,
			int dwFlags);

		// BOOL CryptCreateHash(
		//     [in]   HCRYPTPROV  hProv,
		//     [in]   ALG_ID      Algid,
		//     [in]   HCRYPTKEY   hKey,
		//     [in]   DWORD       dwFlags,
		//     [out]  HCRYPTHASH *phHash
		// );
		[DllImport("advapi32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CryptCreateHash(
			CryptContextHandle hProv,
			int algid,
			IntPtr hKey,
			int dwFlags,
			out CryptHashHandle phHash);

		// BOOL CryptDestroyHash(
		//     [in]  HCRYPTHASH hHash
		// );
		[DllImport("advapi32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CryptDestroyHash(
			IntPtr hHash);

		// BOOL CryptGetHashParam(
		//     [in]      HCRYPTHASH  hHash,
		//     [in]      DWORD       dwParam,
		//     [out]     BYTE       *pbData,
		//     [in, out] DWORD      *pdwDataLen,
		//     [in]      DWORD       dwFlags
		// );
		[DllImport("advapi32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CryptGetHashParam(
			CryptHashHandle hHash,
			int dwParam,
			IntPtr pbData,
			ref int pdwDataLen,
			int dwFlags = 0);

		// BOOL CryptHashData(
		//     [in]  HCRYPTHASH  hHash,
		//     [in]  BYTE       *pbData,
		//     [in]  DWORD       dwDataLen,
		//     [in]  DWORD       dwFlags
		// );
		[DllImport("advapi32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CryptHashData(
			CryptHashHandle hHash,
			IntPtr pbData,
			int dwDataLen,
			int dwFlags = 0);

		// BOOL CryptReleaseContext(
		//     [in]  HCRYPTPROV hProv,
		//     [in]  DWORD      dwFlags
		// );
		[DllImport("advapi32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CryptReleaseContext(
			IntPtr hProv,
			int dwFlags = 0);

		// BOOL WINAPI GetTokenInformation(
		//     [in]      HANDLE                  TokenHandle,
		//     [in]      TOKEN_INFORMATION_CLASS TokenInformationClass,
		//     [out,opt] LPVOID                  TokenInformation,
		//     [in]      DWORD                   TokenInformationLength,
		//     [out]     PDWORD                  ReturnLength
		// );
		[DllImport("advapi32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetTokenInformation(
			TokenHandle TokenHandle,
			TOKEN_INFORMATION_CLASS TokenInformationClass,
			IntPtr TokenInformation,
			int TokenInformationLength,
			out int ReturnLength);

		// BOOL WINAPI LookupAccountSid(
		//     [in,opt]  LPCTSTR       lpSystemName,
		//     [in]      PSID          lpSid,
		//     [out,opt] LPTSTR        lpName,
		//     [inout]   LPDWORD       cchName,
		//     [out,opt] LPTSTR        lpReferencedDomainName,
		//     [inout]   LPDWORD       cchReferencedDomainName,
		//     [out]     PSID_NAME_USE peUse
		// );
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, BestFitMapping = false, ThrowOnUnmappableChar = true, SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool LookupAccountSid(
			[MarshalAs(UnmanagedType.LPTStr)] string lpSystemName,
			IntPtr lpSid,
			[Out, MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpName,
			ref int ccName,
			[Out, MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpReferencedDomainName,
			ref int ccReferencedDomainName,
			out SID_NAME_USE peUse);

		// BOOL WINAPI OpenProcessToken(
		//     [in]  HANDLE  ProcessHandle,
		//     [in]  DWORD   DesiredAccess,
		//     [out] PHANDLE TokenHandle
		// );
		[DllImport("advapi32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool OpenProcessToken(
			ProcessHandle ProcessHandle,
			int DesiredAccess,
			out TokenHandle TokenHandle);
	}
}
