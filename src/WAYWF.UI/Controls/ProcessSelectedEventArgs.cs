// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Windows;
using WAYWF.Api;

namespace WAYWF.UI
{
	class ProcessSelectedEventArgs : RoutedEventArgs
	{
		public ProcessSelectedEventArgs(RoutedEvent routedEvent, object sender, ProcessData process)
			: base(routedEvent, sender)
		{
			if (process == null) throw new ArgumentNullException(nameof(process));

			Process = process;
		}

		public ProcessData Process { get; }

		protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
		{
			if (genericHandler is EventHandler<ProcessSelectedEventArgs> handler)
			{
				handler(genericTarget, this);
			}
			else
			{
				base.InvokeEventHandler(genericHandler, genericTarget);
			}
		}
	}
}
