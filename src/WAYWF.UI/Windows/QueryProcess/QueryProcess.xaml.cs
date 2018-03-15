// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Windows;
using System.Windows.Controls;
using WAYWF.Api;

namespace WAYWF.UI
{
	partial class QueryProcess : Window
	{
		public QueryProcess()
		{
			InitializeComponent();
		}

		public static ProcessData SelectProcess(Window owner)
		{
			var window = new QueryProcess();
			window.Owner = owner;

			if (window.ShowDialog() == true)
			{
				return window._data;
			}

			return null;
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			pidBox.Focus();
		}

		void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (string.IsNullOrEmpty(pidBox.Text))
			{
				_data = null;
			}
			else if (int.TryParse(pidBox.Text, out var value) && (_data == null || _data.ProcessID != value))
			{
				_data = ProcessData.FromPID(value);
				processName.Text = _data?.ProcessName ?? string.Empty;
			}
		}

		void Ok_Button_Click(object sender, RoutedEventArgs e)
		{
			if (int.TryParse(pidBox.Text, out var value) && (_data = ProcessData.FromPID(value)) != null)
			{
				DialogResult = true;
				Close();
			}
			else
			{
				processName.Text = null;
			}
		}

		void Cancel_Button_Click(object sender, RoutedEventArgs e)
		{
			_data = null;
			DialogResult = false;
			Close();
		}

		ProcessData _data;
	}
}
