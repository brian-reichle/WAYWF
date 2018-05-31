// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows;
using WAYWF.UI.VirtualFile;
using WAYWF.UI.Win32;
using IDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace WAYWF.UI
{
	sealed class FileData : IDataObject
	{
		public FileData(VirtualFileBase[] files)
		{
			_files = files;
		}

		public IEnumFORMATETC EnumFormatEtc(DATADIR direction)
		{
			if (direction == DATADIR.DATADIR_GET)
			{
				var list = new[]
				{
					new FORMATETC()
					{
						cfFormat = CF_FILEDESCRIPTORW,
						dwAspect = DVASPECT.DVASPECT_CONTENT,
						lindex = -1,
						tymed = TYMED.TYMED_HGLOBAL,
					},
					new FORMATETC()
					{
						cfFormat = CF_FILEDESCRIPTORA,
						dwAspect = DVASPECT.DVASPECT_CONTENT,
						lindex = -1,
						tymed = TYMED.TYMED_HGLOBAL,
					},
					new FORMATETC()
					{
						cfFormat = CF_FILECONTENTS,
						dwAspect = DVASPECT.DVASPECT_CONTENT,
						lindex = 0,
						tymed = TYMED.TYMED_ISTORAGE,
					},
				};

				return new FormatEnumerator(list);
			}

			throw new NotImplementedException();
		}

		public int GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut)
		{
			formatOut = formatIn;
			formatOut.ptd = IntPtr.Zero;

			if (formatOut.cfFormat != CF_FILECONTENTS && formatIn.lindex != -1)
			{
				return HResults.DV_E_LINDEX;
			}
			else if (formatOut.dwAspect != DVASPECT.DVASPECT_CONTENT)
			{
				formatOut.dwAspect = DVASPECT.DVASPECT_CONTENT;
				return HResults.DATA_S_SAMEFORMATETC;
			}

			return HResults.S_OK;
		}

		public void GetData(ref FORMATETC format, out STGMEDIUM medium)
		{
			medium = default;
			medium.tymed = GetPreferredTYMED(format.tymed);

			if (format.cfFormat == CF_FILEDESCRIPTORW)
			{
				GetFileDescriptorDataW(ref medium, _files);
			}
			else if (format.cfFormat == CF_FILEDESCRIPTORA)
			{
				GetFileDescriptorDataA(ref medium, _files);
			}
			else if (format.cfFormat == CF_FILECONTENTS)
			{
				GetFileContentsData(ref medium, _files[format.lindex]);
			}
			else
			{
				throw Marshal.GetExceptionForHR(HResults.DV_E_FORMATETC);
			}
		}

		public void GetDataHere(ref FORMATETC format, ref STGMEDIUM medium)
		{
			throw new NotSupportedException();
		}

		public int QueryGetData(ref FORMATETC format)
		{
			if (format.dwAspect != DVASPECT.DVASPECT_CONTENT)
			{
				return HResults.DV_E_DVASPECTB;
			}

			switch (format.tymed)
			{
				case TYMED.TYMED_ISTREAM:
				case TYMED.TYMED_HGLOBAL:
					return HResults.DV_E_TYMED;
			}

			if (format.cfFormat == 0)
			{
				return HResults.S_FALSE;
			}
			else if (format.cfFormat == CF_FILECONTENTS || format.cfFormat == CF_FILEDESCRIPTORA || format.cfFormat == CF_FILEDESCRIPTORW)
			{
				return HResults.S_OK;
			}
			else
			{
				return HResults.DV_E_FORMATETC;
			}
		}

		#region IDataObject Members

		int IDataObject.DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection)
		{
			connection = 0;
			return HResults.OLE_E_ADVISENOTSUPPORTED;
		}

		void IDataObject.DUnadvise(int connection)
		{
			throw Marshal.GetExceptionForHR(HResults.OLE_E_ADVISENOTSUPPORTED);
		}

		int IDataObject.EnumDAdvise(out IEnumSTATDATA enumAdvise)
		{
			enumAdvise = null;
			return HResults.OLE_E_ADVISENOTSUPPORTED;
		}

		void IDataObject.SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release)
		{
			throw new NotSupportedException();
		}

		#endregion

		static TYMED GetPreferredTYMED(TYMED tymed)
		{
			if ((tymed & TYMED.TYMED_HGLOBAL) != 0)
			{
				return TYMED.TYMED_HGLOBAL;
			}
			else if ((tymed & TYMED.TYMED_ISTREAM) != 0)
			{
				return TYMED.TYMED_ISTREAM;
			}
			else
			{
				throw Marshal.GetExceptionForHR(HResults.DV_E_TYMED);
			}
		}

		static void GetFileDescriptorDataA(ref STGMEDIUM medium, VirtualFileBase[] files)
		{
			const int FILEDESCRIPTOR_FlagsOffset = 0;
			const int FILEDESCRIPTOR_FilenameOffset = 72;
			const int FILEDESCRIPTOR_FilenameLength = 260;
			const int FILEDESCRIPTOR_Length = FILEDESCRIPTOR_FilenameOffset + FILEDESCRIPTOR_FilenameLength;

			var requiredSize = sizeof(int) + FILEDESCRIPTOR_Length * files.Length;

			using (var hMem = SafeWin32.GlobalAlloc(requiredSize))
			{
				var ptr = NativeMethods.GlobalLock(hMem);
				ptr.Initialize((ulong)requiredSize);

				try
				{
					ptr.Write(0UL, files.Length);
					ulong offset = sizeof(int);

					for (var i = 0; i < files.Length; i++)
					{
						var file = files[i];
						var data = Encoding.Default.GetBytes(file.FileName);

						ptr.Write(offset + FILEDESCRIPTOR_FlagsOffset, 0);
						ptr.WriteArray(offset + FILEDESCRIPTOR_FilenameOffset, data, 0, data.Length);

						offset += FILEDESCRIPTOR_Length;
					}
				}
				finally
				{
					NativeMethods.GlobalUnlock(hMem);
				}

				ApplyTo(ref medium, hMem);
			}
		}

		static void GetFileDescriptorDataW(ref STGMEDIUM medium, VirtualFileBase[] files)
		{
			const int FILEDESCRIPTOR_FlagsOffset = 0;
			const int FILEDESCRIPTOR_FilenameOffset = 72;
			const int FILEDESCRIPTOR_FilenameLength = sizeof(char) * 260;
			const int FILEDESCRIPTOR_Length = FILEDESCRIPTOR_FilenameOffset + FILEDESCRIPTOR_FilenameLength;

			var requiredSize = sizeof(int) + FILEDESCRIPTOR_Length * files.Length;

			using (var hMem = SafeWin32.GlobalAlloc(requiredSize))
			{
				var ptr = NativeMethods.GlobalLock(hMem);
				ptr.Initialize((ulong)requiredSize);

				try
				{
					ptr.Write(0UL, files.Length);
					ulong offset = sizeof(int);

					for (var i = 0; i < files.Length; i++)
					{
						var file = files[i];
						var data = Encoding.Unicode.GetBytes(file.FileName);

						ptr.Write(offset + FILEDESCRIPTOR_FlagsOffset, FD_UNICODE);
						ptr.WriteArray(offset + FILEDESCRIPTOR_FilenameOffset, data, 0, data.Length);

						offset += FILEDESCRIPTOR_Length;
					}
				}
				finally
				{
					NativeMethods.GlobalUnlock(hMem);
				}

				ApplyTo(ref medium, hMem);
			}
		}

		static void GetFileContentsData(ref STGMEDIUM medium, VirtualFileBase file)
		{
			var data = file.GenerateContent();

			using (var hMem = SafeWin32.GlobalAlloc(data.Length))
			{
				var ptr = NativeMethods.GlobalLock(hMem);
				ptr.Initialize((ulong)data.Length);

				try
				{
					ptr.WriteArray(0, data, 0, data.Length);
				}
				finally
				{
					NativeMethods.GlobalUnlock(hMem);
				}

				ApplyTo(ref medium, hMem);
			}
		}

		static void ApplyTo(ref STGMEDIUM medium, SafeHGlobal hMem)
		{
			switch (medium.tymed)
			{
				case TYMED.TYMED_HGLOBAL:
					hMem.TransferOwnershipTo(out medium.unionmember);
					break;

				case TYMED.TYMED_ISTREAM:
					hMem.TransferOwnershipAsStreamTo(out medium.unionmember);
					break;

				default:
					throw new ArgumentException("Unrecognised tymed.");
			}
		}

		const int FD_UNICODE = unchecked((int)0x80000000);

		const string CFSTR_FILEDESCRIPTORA = "FileGroupDescriptor";
		const string CFSTR_FILEDESCRIPTORW = "FileGroupDescriptorW";
		const string CFSTR_FILECONTENTS = "FileContents";

		static readonly short CF_FILEDESCRIPTORA = (short)DataFormats.GetDataFormat(CFSTR_FILEDESCRIPTORA).Id;
		static readonly short CF_FILEDESCRIPTORW = (short)DataFormats.GetDataFormat(CFSTR_FILEDESCRIPTORW).Id;
		static readonly short CF_FILECONTENTS = (short)DataFormats.GetDataFormat(CFSTR_FILECONTENTS).Id;

		readonly VirtualFileBase[] _files;

		sealed class FormatEnumerator : IEnumFORMATETC
		{
			public FormatEnumerator(FORMATETC[] list)
				: this(list, 0)
			{
			}

			FormatEnumerator(FORMATETC[] list, int index)
			{
				_list = list;
				_index = index;
			}

			public void Clone(out IEnumFORMATETC newEnum)
			{
				newEnum = new FormatEnumerator(_list, _index);
			}

			public int Next(int celt, FORMATETC[] rgelt, int[] pceltFetched)
			{
				var available = _list.Length - _index;
				int count;
				int result;

				if (available >= celt)
				{
					count = celt;
					result = HResults.S_OK;
				}
				else if (available > 0)
				{
					count = available;
					result = HResults.S_FALSE;
				}
				else
				{
					count = 0;
					result = HResults.S_FALSE;
					goto coppied;
				}

				Array.Copy(_list, _index, rgelt, 0, count);
			coppied:

				if (pceltFetched != null)
				{
					pceltFetched[0] = count;
				}

				_index += count;
				return result;
			}

			public int Reset()
			{
				_index = 0;
				return HResults.S_OK;
			}

			public int Skip(int celt)
			{
				if (celt + _index <= _list.Length)
				{
					_index += celt;
					return HResults.S_OK;
				}
				else
				{
					_index = _list.Length;
					return HResults.S_FALSE;
				}
			}

			int _index;
			readonly FORMATETC[] _list;
		}
	}
}
