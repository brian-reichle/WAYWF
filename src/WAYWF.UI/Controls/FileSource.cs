// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WAYWF.UI.VirtualFile;
using WAYWF.UI.Win32;

namespace WAYWF.UI
{
	class FileSource : Control
	{
		public static readonly DependencyProperty FileProperty = DependencyProperty.Register(
			nameof(File),
			typeof(VirtualFileBase),
			typeof(FileSource),
			new FrameworkPropertyMetadata(null, OnFileChanged));

		static readonly DependencyPropertyKey FilenamePropertyKey = DependencyProperty.RegisterReadOnly(
			nameof(Filename),
			typeof(string),
			typeof(FileSource),
			new FrameworkPropertyMetadata());

		public static readonly DependencyProperty FilenameProperty = FilenamePropertyKey.DependencyProperty;

		static readonly DependencyPropertyKey IconPropertyKey = DependencyProperty.RegisterReadOnly(
			nameof(Icon),
			typeof(ImageSource),
			typeof(FileSource),
			new FrameworkPropertyMetadata());

		public static readonly DependencyProperty IconProperty = IconPropertyKey.DependencyProperty;

		static FileSource()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(FileSource), new FrameworkPropertyMetadata(typeof(FileSource)));
			FocusableProperty.OverrideMetadata(typeof(FileSource), new FrameworkPropertyMetadata(false));
		}

		public VirtualFileBase File
		{
			get { return (VirtualFileBase)GetValue(FileProperty); }
			set { SetValue(FileProperty, value); }
		}

		public string Filename
		{
			get { return (string)GetValue(FilenameProperty); }
			private set { SetValue(FilenamePropertyKey, value); }
		}

		public ImageSource Icon
		{
			get { return (ImageSource)GetValue(IconProperty); }
			private set { SetValue(IconPropertyKey, value); }
		}

		static void OnFileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = (FileSource)d;
			var file = (VirtualFileBase)e.NewValue;

			if (file == null)
			{
				control.Filename = null;
				control.Icon = null;
			}
			else
			{
				control.Filename = file.FileName;
				control.Icon = SafeWin32.GetIcon(file.Extension);
			}
		}
	}
}
