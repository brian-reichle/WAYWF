// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace WAYWF.UI
{
	sealed class WheelGesture : MouseGesture
	{
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Referenced via xaml.")]
		public static WheelGesture Up = new WheelGesture(true);
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Referenced via xaml.")]
		public static WheelGesture Down = new WheelGesture(false);

		WheelGesture(bool up)
		{
			_matchUp = up;
		}

		public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
		{
			if (inputEventArgs is MouseWheelEventArgs args)
			{
				if (_matchUp)
				{
					return args.Delta > 0;
				}
				else
				{
					return args.Delta < 0;
				}
			}

			return false;
		}

		readonly bool _matchUp;
	}
}
