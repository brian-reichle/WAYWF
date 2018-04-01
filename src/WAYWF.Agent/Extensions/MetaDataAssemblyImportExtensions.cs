// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using WAYWF.Agent.MetaDataApi;

namespace WAYWF.Agent
{
	static class MetaDataAssemblyImportExtensions
	{
		public static unsafe bool GetAssemblyProps(this IMetaDataAssemblyImport aImport, out string name, out Version version, out string locale, out long? publicKeyToken)
		{
			int size;
			var info = new ASSEMBLYMETADATA();

			var hr = aImport.GetAssemblyFromScope(out var assemblyToken);

			if (hr < 0)
			{
				name = null;
				version = null;
				locale = null;
				publicKeyToken = null;
				return false;
			}

			IntPtr publicKeyPtr;
			int publicKeyLen;

			aImport.GetAssemblyProps(
				assemblyToken,
				ppbPublicKey: &publicKeyPtr,
				pcbPublicKey: &publicKeyLen,
				pchName: &size,
				pMetaData: &info);

			version = new Version(info.usMajorVersion, info.usMinorVersion, info.usBuildNumber, info.usRevisionNumber);

			if (info.szLocale == IntPtr.Zero || info.cbLocale <= 1)
			{
				locale = null;
			}
			else
			{
				locale = new string((char*)info.szLocale, 0, info.cbLocale - 1);
			}

			if (publicKeyPtr == IntPtr.Zero)
			{
				publicKeyToken = null;
			}
			else
			{
				publicKeyToken = CryptFunctions.GetPublicKeyToken(publicKeyPtr, publicKeyLen);
			}

			if (size <= 1)
			{
				name = string.Empty;
			}
			else
			{
				var buffer = stackalloc char[size];

				aImport.GetAssemblyProps(
					assemblyToken,
					szName: buffer,
					cchName: size,
					pchName: &size);

				name = new string(buffer, 0, size - 1);
			}

			return true;
		}

		public static unsafe bool GetAssemblyRefProps(this IMetaDataAssemblyImport aImport, MetaDataToken assemblyRefToken, out string name, out Version version, out string locale, out long? publicKeyToken)
		{
			int size;
			var info = new ASSEMBLYMETADATA();

			IntPtr publicKeyPtr;
			int publicKeyLen;

			aImport.GetAssemblyRefProps(
				assemblyRefToken,
				ppbPublicKeyOrToken: &publicKeyPtr,
				pcbPublicKeyOrToken: &publicKeyLen,
				pchName: &size,
				pMetaData: &info);

			version = new Version(info.usMajorVersion, info.usMinorVersion, info.usBuildNumber, info.usRevisionNumber);

			if (info.szLocale == IntPtr.Zero || info.cbLocale <= 1)
			{
				locale = null;
			}
			else
			{
				locale = new string((char*)info.szLocale, 0, info.cbLocale - 1);
			}

			if (publicKeyPtr == IntPtr.Zero)
			{
				publicKeyToken = null;
			}
			else
			{
				publicKeyToken = CryptFunctions.GetPublicKeyToken(publicKeyPtr, publicKeyLen);
			}

			if (size <= 1)
			{
				name = string.Empty;
			}
			else
			{
				var buffer = stackalloc char[size];

				aImport.GetAssemblyRefProps(
					assemblyRefToken,
					szName: buffer,
					cchName: size,
					pchName: &size);

				name = new string(buffer, 0, size - 1);
			}

			return true;
		}

		public static unsafe MetaDataToken GetExportedTypeImplementation(this IMetaDataAssemblyImport aImport, MetaDataToken mdExportedType)
		{
			MetaDataToken implementation;

			aImport.GetExportedTypeProps(
				mdExportedType,
				ptkImplementation: &implementation);

			return implementation;
		}
	}
}
