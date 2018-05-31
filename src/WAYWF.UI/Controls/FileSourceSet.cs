// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WAYWF.UI.VirtualFile;
using WAYWF.UI.Win32;

namespace WAYWF.UI
{
	[TemplatePart(Name = "PART_List", Type = typeof(MultiDragList))]
	class FileSourceSet : Control
	{
		public static readonly DependencyProperty FileListProperty = DependencyProperty.Register(
			nameof(FileList),
			typeof(IEnumerable<VirtualFileBase>),
			typeof(FileSourceSet),
			new FrameworkPropertyMetadata());

		static FileSourceSet()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(FileSourceSet), new FrameworkPropertyMetadata(typeof(FileSourceSet)));
		}

		public FileSourceSet()
		{
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, OnCopyExecuted));
		}

		public IEnumerable<VirtualFileBase> FileList
		{
			get => (IEnumerable<VirtualFileBase>)GetValue(FileListProperty);
			set => SetValue(FileListProperty, value);
		}

		public override void OnApplyTemplate()
		{
			if (_list != null)
			{
				_list.StartDrag -= ListStartDrag;
			}

			base.OnApplyTemplate();

			_list = GetTemplateChild("PART_List") as MultiDragList;

			if (_list != null)
			{
				_list.StartDrag += ListStartDrag;
			}
		}

		static FileData CreateFileData(MultiDragList list)
		{
			return new FileData(list.SelectedItems.Cast<VirtualFileBase>().ToArray());
		}

		void DoDragDrop(object dataObject)
		{
			try
			{
				DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Copy);
			}
			catch (COMException ex) when (ex.ErrorCode == HResults.RPC_S_CALL_FAILED)
			{
			}
		}

		void ListStartDrag(object sender, RoutedEventArgs e)
		{
			if (_list != null)
			{
				DoDragDrop(CreateFileData(_list));
			}
		}

		void OnCopyExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (_list != null)
			{
				e.Handled = true;
				Clipboard.SetDataObject(CreateFileData(_list));
			}
		}

		MultiDragList _list;
	}
}
