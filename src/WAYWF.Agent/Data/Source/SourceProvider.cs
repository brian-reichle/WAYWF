// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using WAYWF.Agent.CorDebugApi;
using WAYWF.Agent.Data;
using WAYWF.Agent.SymbolStoreApi;

namespace WAYWF.Agent.Source
{
	sealed class SourceProvider : IDisposable
	{
		public SourceProvider()
		{
			_documentIdentities = Identity.NewSource();
			_readerCache = new Dictionary<ICorDebugModule, ModuleInfo>();
			_documentPathCache = new Dictionary<string, SourceDocument>();
			_documentCache = new Dictionary<ISymUnmanagedDocument, SourceDocument>();
		}

		public void Dispose()
		{
			if (_isDisposed)
			{
				_isDisposed = true;

				foreach (var info in _readerCache.Values)
				{
					if (info != null)
					{
						((ISymUnmanagedDispose)info.Reader).Destroy();
						Marshal.FinalReleaseComObject(info.Reader);
					}
				}

				_readerCache.Clear();

				foreach (var document in _documentCache.Keys)
				{
					Marshal.FinalReleaseComObject(document);
				}

				_documentCache.Clear();

				Marshal.FinalReleaseComObject(_binder);
				_binder = null;
			}
		}

		public SourceRef GetSourceRef(ICorDebugModule module, MetaDataToken token, int offset)
		{
			var tmp = GetModuleInfo(module);

			if (tmp == null)
			{
				return null;
			}

			return GetSourceRef(tmp, token, offset);
		}

		public SourceAsyncState GetAsyncSourceRef(ICorDebugModule module, MetaDataToken token, int state)
		{
			if (state < 0)
			{
				return null;
			}

			var tmp = GetModuleInfo(module);

			if (tmp == null)
			{
				return null;
			}

			return GetAsyncSourceRef(tmp, token, state);
		}

		public string[] GetLocalNames(ICorDebugModule module, MetaDataToken token, int ilOffset, int localCount)
		{
			if (localCount == 0)
			{
				return null;
			}

			var info = GetModuleInfo(module);

			if (info == null)
			{
				return null;
			}

			var method = info.Reader.GetMethod(token);
			return method.GetVariableNames(ilOffset, localCount);
		}

		public SourceDocument[] GetAllDocuments()
		{
			var documents = _documentPathCache.Values;
			var result = new SourceDocument[documents.Count];
			documents.CopyTo(result, 0);
			return result;
		}

		SourceRef GetSourceRef(ModuleInfo tmp, MetaDataToken token, int offset)
		{
			if (!tmp.Methods.TryGetValue(token, out var result))
			{
				result = GetMethodInfo(tmp, token);
				tmp.Methods.Add(token, result);
			}

			return result[offset];
		}

		SourceAsyncState GetAsyncSourceRef(ModuleInfo tmp, MetaDataToken token, int state)
		{
			if (!tmp.AsyncMethods.TryGetValue(token, out var result))
			{
				result = GetAsyncMethodInfo(tmp, token);
				tmp.AsyncMethods.Add(token, result);
			}

			return result?[state];
		}

		MethodInfo GetMethodInfo(ModuleInfo module, MetaDataToken token)
		{
			var method = module.Reader.GetMethod(token);
			var count = method.GetSequencePointCount();

			if (count < 0)
			{
				return null;
			}

			var offsets = new int[count];
			var documents = new ISymUnmanagedDocument[count];
			var fromLines = new int[count];
			var toLines = new int[count];
			var fromColumns = new int[count];
			var toColumns = new int[count];
			var refs = new SourceRef[count];

			method.GetSequencePoints(count, out count, offsets, documents, fromLines, fromColumns, toLines, toColumns);

			for (var i = 0; i < refs.Length; i++)
			{
				if (fromLines[i] != HiddenLine)
				{
					refs[i] = new SourceRef(
						GetDocument(documents[i]),
						fromLines[i],
						toLines[i],
						fromColumns[i],
						toColumns[i]);
				}
			}

			return new MethodInfo(offsets, refs);
		}

		AsyncMethodInfo GetAsyncMethodInfo(ModuleInfo module, MetaDataToken token)
		{
			var method = module.Reader.GetMethod(token);

			if (!(method is ISymUnmanagedAsyncMethod aMethod) || !aMethod.IsAsyncMethod())
			{
				return null;
			}

			var count = aMethod.GetAsyncStepInfoCount();
			var offsets = new int[count];
			var bpOffsets = new int[count];
			var bpMethods = new MetaDataToken[count];

			aMethod.GetAsyncStepInfo(count, out count, offsets, bpOffsets, bpMethods);

			var result = new SourceAsyncState[count];

			for (var i = 0; i < result.Length; i++)
			{
				result[i] = new SourceAsyncState(
					offsets[i],
					GetSourceRef(module, token, offsets[i]));
			}

			return new AsyncMethodInfo(result);
		}

		SourceDocument GetDocument(ISymUnmanagedDocument document)
		{
			if (!_documentCache.TryGetValue(document, out var result))
			{
				var url = document.GetURL();

				if (!_documentPathCache.TryGetValue(url, out result))
				{
					result = new SourceDocument(
						_documentIdentities.New(),
						url,
						document.GetLanguage(),
						document.GetDocumentType());

					_documentPathCache.Add(url, result);
				}

				_documentCache.Add(document, result);
			}

			return result;
		}

		ModuleInfo GetModuleInfo(ICorDebugModule module)
		{
			if (!_readerCache.TryGetValue(module, out var info))
			{
				var reader = GetReader(module);

				if (reader != null)
				{
					_readerCache.Add(module, info = new ModuleInfo(reader));
				}
			}

			return info;
		}

		ISymUnmanagedReader GetReader(ICorDebugModule module)
		{
			if (module.IsDynamic() || module.IsInMemory())
			{
				if (module is ICorDebugModule3 module3)
				{
					module3.CreateReaderForInMemorySymbols();
				}

				return null;
			}

			if (_binder == null)
			{
				_binder = (ISymUnmanagedBinder)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID.CLSID_CorSymBinder));
			}

			var hr = _binder.GetReaderForFile(
				module.GetMetaDataImport(),
				module.GetName(),
				null,
				out var reader);

			if (hr == HResults.E_PDB_NOT_FOUND || hr == HResults.E_PDB_CORRUPT)
			{
				return null;
			}
			else if (hr < 0)
			{
				throw Marshal.GetExceptionForHR(hr);
			}

			return reader;
		}

		const int HiddenLine = 0x00FeeFee;

		bool _isDisposed;
		ISymUnmanagedBinder _binder;
		readonly IIdentitySource _documentIdentities;
		readonly Dictionary<ICorDebugModule, ModuleInfo> _readerCache;
		readonly Dictionary<string, SourceDocument> _documentPathCache;
		readonly Dictionary<ISymUnmanagedDocument, SourceDocument> _documentCache;

		sealed class ModuleInfo
		{
			public ModuleInfo(ISymUnmanagedReader reader)
			{
				Reader = reader;
				Methods = new Dictionary<MetaDataToken, MethodInfo>();
				AsyncMethods = new Dictionary<MetaDataToken, AsyncMethodInfo>();
			}

			public ISymUnmanagedReader Reader { get; }
			public Dictionary<MetaDataToken, MethodInfo> Methods { get; }
			public Dictionary<MetaDataToken, AsyncMethodInfo> AsyncMethods { get; }
		}

		sealed class MethodInfo
		{
			public MethodInfo(int[] ilOffsets, SourceRef[] refs)
			{
				_ilOffsets = ilOffsets;
				_refs = refs;
			}

			public SourceRef this[int offset]
			{
				get
				{
					var index = Array.BinarySearch(_ilOffsets, offset);

					if (index < 0)
					{
						index = ~index;
						// at this point, index is where the value would be inserted ... but we want the
						// largest value less than offset, which means we need to subtract 1.
						index--;
					}

					return index >= 0 && index < _refs.Length ? _refs[index] : null;
				}
			}

			readonly int[] _ilOffsets;
			readonly SourceRef[] _refs;
		}

		sealed class AsyncMethodInfo
		{
			public AsyncMethodInfo(SourceAsyncState[] states)
			{
				_states = states;
			}

			public SourceAsyncState this[int state] => _states[state];

			readonly SourceAsyncState[] _states;
		}
	}
}
