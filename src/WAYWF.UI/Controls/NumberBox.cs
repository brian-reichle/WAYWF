// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WAYWF.UI
{
	class NumberBox : TextBox
	{
		public NumberBox()
		{
			DataObject.AddPastingHandler(this, OnPasting);
			InputScope = new InputScope()
			{
				Names =
				{
					new InputScopeName()
					{
						NameValue = InputScopeNameValue.Number,
					}
				},
			};
		}

		protected override void OnTextInput(TextCompositionEventArgs e)
		{
			if (!IsNumeric(e.Text))
			{
				e.Handled = true;
			}

			base.OnTextInput(e);
		}

		void OnPasting(object sender, DataObjectPastingEventArgs e)
		{
			if (!e.CommandCancelled &&
				e.DataObject.GetDataPresent(DataFormats.Text) &&
				e.DataObject.GetData(DataFormats.Text) is string text &&
				IsNumeric(text))
			{
				return;
			}

			e.CancelCommand();
		}

		static bool IsNumeric(string text)
		{
			for (var i = 0; i < text.Length; i++)
			{
				if (!char.IsDigit(text[i]))
				{
					return false;
				}
			}

			return true;
		}
	}
}
