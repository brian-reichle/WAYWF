// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Windows;
using System.Windows.Input;

namespace WAYWF.UI
{
	public partial class HelpWindow : Window
	{
		public HelpWindow()
		{
			InitializeComponent();
			DataContext = new HelpData();
			CloseButton.Focus();
		}

		void OnCloseExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			Close();
		}
	}
}
