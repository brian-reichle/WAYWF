// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WAYWF.UI
{
	[TemplatePart(Type = typeof(TextBox), Name = "PART_TextBox")]
	class Spinner : Control
	{
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
			nameof(Value),
			typeof(int),
			typeof(Spinner),
			new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged, OnCoerceValue));

		public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
			nameof(MinValue),
			typeof(int),
			typeof(Spinner),
			new FrameworkPropertyMetadata(0, OnMinValueChanged));

		public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
			nameof(MaxValue),
			typeof(int),
			typeof(Spinner),
			new FrameworkPropertyMetadata(int.MaxValue, OnMaxValueChanged, OnCoerceMaxValue));

		public static readonly RoutedCommand RollUp = new RoutedCommand(nameof(RollUp), typeof(Spinner));
		public static readonly RoutedCommand RollDown = new RoutedCommand(nameof(RollDown), typeof(Spinner));

		static Spinner()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Spinner), new FrameworkPropertyMetadata(typeof(Spinner)));
			CommandManager.RegisterClassCommandBinding(typeof(Spinner), new CommandBinding(RollUp, OnRollUpExecuted));
			CommandManager.RegisterClassCommandBinding(typeof(Spinner), new CommandBinding(RollDown, OnRollDownExecuted));
		}

		public int Value
		{
			get => (int)GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		public int MinValue
		{
			get => (int)GetValue(MinValueProperty);
			set => SetValue(MinValueProperty, value);
		}

		public int MaxValue
		{
			get => (int)GetValue(MaxValueProperty);
			set => SetValue(MaxValueProperty, value);
		}

		public override void OnApplyTemplate()
		{
			if (_textBox != null)
			{
				_textBox.TextChanged -= new TextChangedEventHandler(TextBoxTextChanged);
			}

			base.OnApplyTemplate();

			_textBox = (TextBox)GetTemplateChild("PART_TextBox");

			if (_textBox != null)
			{
				_textBox.Text = Value.ToString();
				_textBox.TextChanged += new TextChangedEventHandler(TextBoxTextChanged);

				if (_textBox.IsFocused)
				{
					_textBox.Focus();
				}
			}
		}

		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			if (!e.Handled && e.NewFocus == this)
			{
				e.Handled = true;
				_textBox.Focus();
			}

			base.OnGotKeyboardFocus(e);
		}

		static void OnMinValueChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var control = (Spinner)sender;
			control.CoerceValue(MaxValueProperty);
			control.CoerceValue(ValueProperty);
		}

		static void OnMaxValueChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var control = (Spinner)sender;
			control.CoerceValue(ValueProperty);
		}

		static void OnValueChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var control = (Spinner)sender;
			var tb = control._textBox;

			if (tb != null)
			{
				tb.Text = e.NewValue.ToString();
			}
		}

		static object OnCoerceValue(object sender, object baseValue)
		{
			var control = (Spinner)sender;
			var value = (int)baseValue;
			var tmp = control.ClampValue(value);
			return tmp == value ? baseValue : tmp;
		}

		static object OnCoerceMaxValue(object sender, object baseValue)
		{
			var control = (Spinner)sender;
			return control.MinValue > (int)baseValue ? control.MinValue : baseValue;
		}

		static void OnRollUpExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			var command = (Spinner)sender;
			command.Value++;
		}

		static void OnRollDownExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			var command = (Spinner)sender;
			command.Value--;
		}

		void TextBoxTextChanged(object sender, TextChangedEventArgs e)
		{
			var tb = (TextBox)sender;

			if (string.IsNullOrEmpty(tb.Text))
			{
				Value = 0;
			}
			else if (int.TryParse(tb.Text, out var value))
			{
				var tmp = ClampValue(value);

				if (tmp != value)
				{
					tb.Text = tmp.ToString();
				}
				else
				{
					Value = value;
				}
			}
			else
			{
				tb.Text = Value.ToString();
			}
		}

		int ClampValue(int value)
		{
			if (value < MinValue)
			{
				return MinValue;
			}
			else if (value > MaxValue)
			{
				return MaxValue;
			}
			else
			{
				return value;
			}
		}

		TextBox _textBox;
	}
}
