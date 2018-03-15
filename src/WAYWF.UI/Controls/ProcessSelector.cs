// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using WAYWF.Api;
using WAYWF.UI.Win32;

namespace WAYWF.UI
{
	[TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
	sealed class ProcessSelector : Control
	{
		static readonly DependencyPropertyKey IsDraggingPropertyKey = DependencyProperty.RegisterReadOnly(
			nameof(IsDragging),
			typeof(bool),
			typeof(ProcessSelector),
			new FrameworkPropertyMetadata(false));

		public static readonly DependencyProperty IsDraggingProperty = IsDraggingPropertyKey.DependencyProperty;

		static readonly DependencyPropertyKey CurrentProcessPropertyKey = DependencyProperty.RegisterReadOnly(
			nameof(CurrentProcess),
			typeof(ProcessData),
			typeof(ProcessSelector),
			new FrameworkPropertyMetadata());

		public static readonly DependencyProperty CurrentProcessProperty = CurrentProcessPropertyKey.DependencyProperty;

		public static readonly RoutedEvent ProcessSelectedEvent = EventManager.RegisterRoutedEvent(
			nameof(ProcessSelected),
			RoutingStrategy.Bubble,
			typeof(EventHandler<ProcessSelectedEventArgs>),
			typeof(ProcessSelector));

		static ProcessSelector()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ProcessSelector), new FrameworkPropertyMetadata(typeof(ProcessSelector)));
		}

		public bool IsDragging
		{
			get { return (bool)GetValue(IsDraggingProperty); }
			private set { SetValue(IsDraggingPropertyKey, value); }
		}

		public ProcessData CurrentProcess
		{
			get { return (ProcessData)GetValue(CurrentProcessProperty); }
			private set { SetValue(CurrentProcessPropertyKey, value); }
		}

		public event EventHandler<ProcessSelectedEventArgs> ProcessSelected
		{
			add { AddHandler(ProcessSelectedEvent, value); }
			remove { RemoveHandler(ProcessSelectedEvent, value); }
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_popup = (Popup)GetTemplateChild("PART_Popup");

			if (_popup != null)
			{
				_popup.Placement = PlacementMode.Relative;
			}
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			if (e.MouseDevice.Capture(this))
			{
				_start = e.GetPosition(this);

				e.Handled = true;
				return;
			}

			base.OnMouseLeftButtonDown(e);
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			if (IsMouseCaptured)
			{
				e.Handled = true;
				var tmp = CurrentProcess;
				EndDragging();
				ReleaseMouseCapture();

				if (tmp != null)
				{
					OnProcessSelected(tmp);
				}
				return;
			}

			base.OnMouseLeftButtonUp(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			var current = e.GetPosition(this);

			if (e.LeftButton == MouseButtonState.Pressed && IsMouseCaptured)
			{
				e.Handled = true;

				if (!IsDragging)
				{
					var vector = current - _start;

					if (Math.Abs(vector.X) > SystemParameters.MinimumHorizontalDragDistance ||
						Math.Abs(vector.Y) > SystemParameters.MinimumVerticalDragDistance)
					{
						BeginDragging();
						UpdatePopupPlacement(current);
					}
				}
				else
				{
					UpdatePopupPlacement(current);

					var pointOnScreen = PointToScreen(current);
					var pid = SafeWin32.GetProcessIDAtPoint((int)pointOnScreen.X, (int)pointOnScreen.Y);

					if (!pid.HasValue)
					{
						CurrentProcess = null;
					}
					else
					{
						var data = CurrentProcess;

						if (data == null || data.ProcessID != pid.Value)
						{
							CurrentProcess = ProcessData.FromPID(pid.Value);
						}
					}
				}

				return;
			}

			base.OnMouseMove(e);
		}

		void UpdatePopupPlacement(Point current)
		{
			const int PopupDeltaX = 16;
			const int PopupDeltaY = 16;

			_popup.HorizontalOffset = current.X + PopupDeltaX;
			_popup.VerticalOffset = current.Y + PopupDeltaY;
		}

		void BeginDragging()
		{
			CurrentProcess = null;
			IsDragging = true;
			InputManager.Current.PreProcessInput += PreProcessDraggingInput;
		}

		void EndDragging()
		{
			CurrentProcess = null;
			IsDragging = false;
			InputManager.Current.PreProcessInput -= PreProcessDraggingInput;
		}

		void PreProcessDraggingInput(object sender, PreProcessInputEventArgs e)
		{
			if (e.StagingItem.Input is KeyEventArgs args && args.Key == Key.Escape)
			{
				EndDragging();
				ReleaseMouseCapture();
				e.Cancel();
			}
		}

		void OnProcessSelected(ProcessData process)
		{
			RaiseEvent(new ProcessSelectedEventArgs(ProcessSelectedEvent, this, process));
		}

		Point _start;
		Popup _popup;
	}
}
