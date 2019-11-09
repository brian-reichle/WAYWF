// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace WAYWF.Agent.Core
{
	static class HResults
	{
		public const int E_FAIL = unchecked((int)0x80004005);
		public const int E_BUFFER_TOO_SMALL = unchecked((int)0x8007007A);
		public const int E_PDB_NOT_FOUND = unchecked((int)0x806D0005);
		public const int E_PDB_CORRUPT = unchecked((int)0x806D0014);
		public const int E_ACCESSDENIED = unchecked((int)0x80070005);
		public const int E_INVALIDARG = unchecked((int)0x80070057);

		public const int CLDB_E_RECORD_NOTFOUND = unchecked((int)0x80131130);

		public const int CORDBG_E_PROCESS_TERMINATED = unchecked((int)0x80131301);
		public const int CORDBG_E_CLASS_NOT_LOADED = unchecked((int)0x80131303);
		public const int CORDBG_E_IL_VAR_NOT_AVAILABLE = unchecked((int)0x80131304);
		public const int CORDBG_E_BAD_REFERENCE_VALUE = unchecked((int)0x80131305);
		public const int CORDBG_E_FIELD_NOT_AVAILABLE = unchecked((int)0x80131306);
		public const int CORDBG_E_BAD_THREAD_STATE = unchecked((int)0x8013132D);
		public const int CORDBG_E_DEBUGGER_ALREADY_ATTACHED = unchecked((int)0x8013132E);
		public const int CORDBG_E_THREAD_NOT_SCHEDULED = unchecked((int)0x80131C00);
		public const int CORDBG_E_CANNOT_RESOLVE_ASSEMBLY = unchecked((int)0x80131C11);
		public const int CORDBG_E_SYMBOLS_NOT_AVAILABLE = unchecked((int)0x80131C3B);
		public const int CORDBG_E_READVIRTUAL_FAILURE = unchecked((int)0x80131C49);

		public const int ERROR_GEN_FAILURE = unchecked((int)0x8007001F);
		public const int ERROR_PARTIAL_COPY = unchecked((int)0x8007012B);
	}
}
