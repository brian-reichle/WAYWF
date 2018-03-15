// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using WAYWF.Agent.Source;
using WAYWF.Agent.SymbolStoreApi;

namespace WAYWF.Agent
{
	static class SymDocumentExtensions
	{
		public static unsafe string GetURL(this ISymUnmanagedDocument document)
		{
			document.GetURL(0, out var size, null);

			if (size <= 1)
			{
				return null;
			}

			var buffer = stackalloc char[size];
			document.GetURL(size, out size, buffer);
			return new string(buffer, 0, size - 1);
		}

		public static SourceLanguage GetLanguage(this ISymUnmanagedDocument document)
		{
			document.GetLanguageVendor(out var languageVendor);

			if (languageVendor == SymGuids.CorSym_LanguageVendor_Microsoft)
			{
				document.GetLanguage(out var language);

				if (_languageLookup.TryGetValue(language, out var result))
				{
					return result;
				}
			}

			return SourceLanguage.Unknown;
		}

		public static SourceDocumentType GetDocumentType(this ISymUnmanagedDocument document)
		{
			document.GetDocumentType(out var documentType);

			if (documentType == SymGuids.CorSym_DocumentType_Text)
			{
				return SourceDocumentType.Text;
			}
			else if (documentType == SymGuids.CorSym_DocumentType_MC)
			{
				return SourceDocumentType.MC;
			}
			else
			{
				return SourceDocumentType.Unknown;
			}
		}

		static readonly Dictionary<Guid, SourceLanguage> _languageLookup = new Dictionary<Guid, SourceLanguage>()
		{
#pragma warning disable SA1001 // Commas must be spaced correctly
#pragma warning disable SA1025 // Code must not contain multiple whitespace in a row
			{ SymGuids.CorSym_LanguageType_C         , SourceLanguage.C          },
			{ SymGuids.CorSym_LanguageType_CPlusPlus , SourceLanguage.CPlusPlus  },
			{ SymGuids.CorSym_LanguageType_CSharp    , SourceLanguage.CSharp     },
			{ SymGuids.CorSym_LanguageType_Basic     , SourceLanguage.Basic      },
			{ SymGuids.CorSym_LanguageType_Java      , SourceLanguage.Java       },
			{ SymGuids.CorSym_LanguageType_Cobol     , SourceLanguage.Cobol      },
			{ SymGuids.CorSym_LanguageType_Pascal    , SourceLanguage.Pascal     },
			{ SymGuids.CorSym_LanguageType_ILAssembly, SourceLanguage.ILAssembly },
			{ SymGuids.CorSym_LanguageType_JScript   , SourceLanguage.JScript    },
			{ SymGuids.CorSym_LanguageType_SMC       , SourceLanguage.SMC        },
			{ SymGuids.CorSym_LanguageType_MCPlusPlus, SourceLanguage.MCPlusPlus },
#pragma warning restore SA1001 // Commas must be spaced correctly
#pragma warning restore SA1025 // Code must not contain multiple whitespace in a row
		};
	}
}
