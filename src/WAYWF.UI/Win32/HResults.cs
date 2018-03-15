// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace WAYWF.UI.Win32
{
	static class HResults
	{
		public const int S_OK = 0;
		public const int S_FALSE = 1;
		public const int E_NOTIMPL = unchecked((int)0x80004001);
		public const int OLE_E_ADVISENOTSUPPORTED = unchecked((int)0x80040003);
		public const int RPC_S_CALL_FAILED = unchecked((int)0x800706BE);
		public const int DV_E_FORMATETC = unchecked((int)0x80040064);
		public const int DV_E_LINDEX = unchecked((int)0x80040068);
		public const int DV_E_TYMED = unchecked((int)0x80040069);
		public const int DV_E_DVASPECTB = unchecked((int)0x8004006B);
		public const int DATA_S_SAMEFORMATETC = 0x00040130;
	}
}
