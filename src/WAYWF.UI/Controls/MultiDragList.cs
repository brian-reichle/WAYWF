// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace WAYWF.UI
{
	class MultiDragList : MultiSelector
	{
		public static readonly RoutedEvent StartDragEvent = EventManager.RegisterRoutedEvent(
			nameof(StartDrag),
			RoutingStrategy.Bubble,
			typeof(RoutedEventHandler),
			typeof(MultiDragList));

		static MultiDragList()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiDragList), new FrameworkPropertyMetadata(typeof(MultiDragList)));
		}

		public event RoutedEventHandler StartDrag
		{
			add => AddHandler(StartDragEvent, value);
			remove => RemoveHandler(StartDragEvent, value);
		}

		protected override DependencyObject GetContainerForItemOverride() => new MultiDragListItem();

		internal void OnMouseLeftButtonDown(MultiDragListItem item, MouseButtonEventArgs e)
		{
			if (!item.IsMouseCaptured)
			{
				const ModifierKeys Mask = ModifierKeys.Control | ModifierKeys.Shift;
				var modifiers = Keyboard.Modifiers & Mask;
				_releaseAction = ReleaseAction.None;

				if (modifiers == ModifierKeys.None)
				{
					if (!item.IsSelected)
					{
						SetSelectedItem(item);
					}
					else
					{
						_releaseAction = ReleaseAction.SingleSelect;
					}
				}
				else if (modifiers == ModifierKeys.Control)
				{
					_releaseAction = item.IsSelected ? ReleaseAction.Unselect : ReleaseAction.Select;
				}

				_mouseDown = e.GetPosition(this);
				item.CaptureMouse();
			}
		}

		internal void OnMouseLeftButtonUp(MultiDragListItem item)
		{
			if (item.IsMouseCaptured)
			{
				item.ReleaseMouseCapture();
				_mouseDown = default;

				switch (_releaseAction)
				{
					case ReleaseAction.Select:
						SelectedItems.Add(item.Content);
						break;

					case ReleaseAction.Unselect:
						SelectedItems.Remove(item.Content);
						break;

					case ReleaseAction.SingleSelect:
						SetSelectedItem(item);
						break;
				}
			}
		}

		internal void OnMouseRightButtonDown(MultiDragListItem item)
		{
			const ModifierKeys Mask = ModifierKeys.Control | ModifierKeys.Shift;

			if (!item.IsSelected && (Keyboard.Modifiers & Mask) != ModifierKeys.Control)
			{
				SetSelectedItem(item);
			}
		}

		internal void OnMouseMove(MultiDragListItem item, MouseEventArgs e)
		{
			if (item.IsMouseCaptured)
			{
				var diff = e.GetPosition(this) - _mouseDown;

				if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
					Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
				{
					item.ReleaseMouseCapture();
					SelectedItems.Add(item.Content);
					OnStartDrag();
				}
			}
		}

		void SetSelectedItem(MultiDragListItem item)
		{
			BeginUpdateSelectedItems();
			try
			{
				SelectedItems.Clear();
				SelectedItems.Add(item.Content);
			}
			finally
			{
				EndUpdateSelectedItems();
			}
		}

		void OnStartDrag()
		{
			RaiseEvent(new RoutedEventArgs(StartDragEvent, this));
		}

		ReleaseAction _releaseAction;
		Point _mouseDown;

		enum ReleaseAction
		{
			None,
			Select,
			Unselect,
			SingleSelect,
		}
	}
}
