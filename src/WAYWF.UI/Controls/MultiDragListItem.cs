// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace WAYWF.UI
{
	class MultiDragListItem : ContentControl
	{
		public static readonly DependencyProperty IsSelectedProperty = Selector.IsSelectedProperty.AddOwner(typeof(MultiDragListItem));

		static MultiDragListItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiDragListItem), new FrameworkPropertyMetadata(typeof(MultiDragListItem)));
		}

		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			if (!e.Handled)
			{
				e.Handled = true;
				ParentList?.OnMouseLeftButtonDown(this, e);
			}

			base.OnMouseLeftButtonDown(e);
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			if (!e.Handled)
			{
				e.Handled = true;
				ParentList?.OnMouseLeftButtonUp(this);
			}

			base.OnMouseLeftButtonUp(e);
		}

		protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
		{
			if (!e.Handled)
			{
				ParentList?.OnMouseRightButtonDown(this);
			}

			base.OnMouseRightButtonDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (!e.Handled)
			{
				e.Handled = true;
				ParentList?.OnMouseMove(this, e);
			}

			base.OnMouseMove(e);
		}

		MultiDragList ParentList
		{
			get { return ItemsControl.ItemsControlFromItemContainer(this) as MultiDragList; }
		}
	}
}
