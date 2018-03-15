// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace WAYWF.UI.VirtualFile
{
	[DebuggerDisplay("Count = {Count}")]
	sealed class VirtualFileSet : IList<VirtualFileBase>, INotifyPropertyChanged, INotifyCollectionChanged
	{
		public string BaseFileName
		{
			[DebuggerStepThrough]
			get => _baseFileName;
			set
			{
				if (_baseFileName != value)
				{
					_baseFileName = value;
					Regenerate();
				}
			}
		}

		public string XmlContent
		{
			[DebuggerStepThrough]
			get => _xmlContent;
			set
			{
				if (_xmlContent != value)
				{
					_xmlContent = value;
					Regenerate();
				}
			}
		}

		public int Count => _files?.Length ?? 0;

		public VirtualFileBase this[int index]
		{
			get
			{
				var tmp = _files;

				if (tmp == null || index >= tmp.Length || index < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(index));
				}

				return tmp[index];
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public int IndexOf(VirtualFileBase item) => item == null ? -1 : Array.IndexOf(_files, item);
		public bool Contains(VirtualFileBase item) => IndexOf(item) >= 0;

		public void CopyTo(VirtualFileBase[] array, int arrayIndex)
		{
			var tmp = _files;

			if (tmp != null)
			{
				Array.Copy(tmp, 0, array, arrayIndex, tmp.Length);
			}
		}

		public IEnumerator<VirtualFileBase> GetEnumerator()
		{
			var tmp = _files;

			if (tmp != null)
			{
				return ((IEnumerable<VirtualFileBase>)tmp).GetEnumerator();
			}

			return Enumerable.Empty<VirtualFileBase>().GetEnumerator();
		}

		#region IList<VirtualFileBase> Members

		void IList<VirtualFileBase>.Insert(int index, VirtualFileBase item) => throw new NotSupportedException();
		void IList<VirtualFileBase>.RemoveAt(int index) => throw new NotSupportedException();

		VirtualFileBase IList<VirtualFileBase>.this[int index]
		{
			[DebuggerStepThrough]
			get => this[index];
			set => throw new NotSupportedException();
		}

		#endregion

		#region ICollection<VirtualFileBase> Members

		void ICollection<VirtualFileBase>.Add(VirtualFileBase item) => throw new NotSupportedException();
		void ICollection<VirtualFileBase>.Clear() => throw new NotSupportedException();

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool ICollection<VirtualFileBase>.IsReadOnly => true;

		bool ICollection<VirtualFileBase>.Remove(VirtualFileBase item) => throw new NotSupportedException();

		#endregion

		#region IEnumerable Members

		[DebuggerStepThrough]
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		#endregion

		void Regenerate()
		{
			var oldFiles = _files;

			_files = Generate(_baseFileName, _xmlContent);

			if (_files != oldFiles)
			{
				OnCollectionChanged();

				if (oldFiles == null || oldFiles.Length != _files.Length)
				{
					OnPropertyChanged(nameof(Count));
				}
			}
		}

		static VirtualFileBase[] Generate(string baseFileName, string xmlContent)
		{
			if (string.IsNullOrEmpty(baseFileName) || string.IsNullOrEmpty(xmlContent))
			{
				return _defaultFileList;
			}
			else
			{
				return new VirtualFileBase[]
				{
					new XmlVirtualFile(baseFileName, xmlContent),
					new HtmlVirtualFile(baseFileName, xmlContent),
					TransformVirtualData.Instance,
				};
			}
		}

		void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		void OnCollectionChanged()
		{
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		string _baseFileName;
		string _xmlContent;
		VirtualFileBase[] _files = _defaultFileList;

		static readonly VirtualFileBase[] _defaultFileList = new VirtualFileBase[]
		{
			TransformVirtualData.Instance,
		};
	}
}
