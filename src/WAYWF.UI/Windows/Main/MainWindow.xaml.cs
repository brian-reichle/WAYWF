// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using WAYWF.Api;
using WAYWF.UI.VirtualFile;

namespace WAYWF.UI
{
	public partial class MainWindow : Window
	{
		public static readonly RoutedCommand QueryProcess = new RoutedCommand(nameof(QueryProcess), typeof(MainWindow));

		public MainWindow()
		{
			InitializeComponent();
			FilesListBox.FileList = _virtualFiles;
			Log.Focus();
		}

		void Write(string line)
		{
			if (Dispatcher.CheckAccess())
			{
				WriteCore(line);
			}
			else
			{
				Dispatcher.BeginInvoke((Action<string>)WriteCore, line);
			}
		}

		void WriteCore(string line)
		{
			Log.AppendText(line);
			Log.AppendText(Environment.NewLine);
		}

		void OnCloseExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			Close();
		}

		void OnSaveExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			var saveDialog = new SaveFileDialog()
			{
				Filter = "Xml File (*.xml)|*.xml|Transformed Html (*.html)|*.html",
				AddExtension = true,
				CreatePrompt = true,
				OverwritePrompt = true,
				ValidateNames = true,
				FileName = _suggestedFilename,
			};

			if (saveDialog.ShowDialog(this).GetValueOrDefault())
			{
				if (Path.GetExtension(saveDialog.FileName) == ".html")
				{
					WriteTranslated(saveDialog.FileName, Output.Text);
				}
				else
				{
					File.WriteAllText(saveDialog.FileName, Output.Text);
				}
			}
		}

		async void ProcessSelector_ProcessSelected(object sender, ProcessSelectedEventArgs e)
		{
			await CaptureProcessDetailsAsync(e.Process);
		}

		async void OnQueryProcessExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			var process = UI.QueryProcess.SelectProcess(this);

			if (process != null)
			{
				await CaptureProcessDetailsAsync(process);
			}
		}

		void OnHelpExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			var window = new HelpWindow() { Owner = this };
			window.ShowDialog();
		}

		void BringTabIntoView(object sender, RequestBringIntoViewEventArgs e)
		{
			var tab = (TabItem)sender;
			var tabControl = (TabControl)tab.Parent;
			tabControl.SelectedItem = tab;
		}

		static void WriteTranslated(string path, string xmlContents)
		{
			using (var file = new FileStream(path, FileMode.Create))
			{
				HtmlTranslator.Transform(xmlContents, file);
			}
		}

		static string SuggestFilename(ProcessData data) => data.ProcessName + "-" + data.ProcessID;

		async Task CaptureProcessDetailsAsync(ProcessData process)
		{
			Log.Text = string.Empty;
			Log.BringIntoView();
			Output.Text = string.Empty;

			var config = new CaptureConfig()
			{
				WalkHeap = WalkHeapButton.IsChecked.GetValueOrDefault(),
				WaitSeconds = WaitSpinner.Value,
				Verbose = true,
			};

			_suggestedFilename = SuggestFilename(process);
			_virtualFiles.BaseFileName = _suggestedFilename;

			var res = await Launcher.CaptureAsync(process, config, Write);

			Output.Text = res.StandardOutput;
			_virtualFiles.XmlContent = res.StandardOutput;

			if (res.ExitCode == 0)
			{
				Output.BringIntoView();
			}
		}

		string _suggestedFilename;
		readonly VirtualFileSet _virtualFiles = new VirtualFileSet();
	}
}
