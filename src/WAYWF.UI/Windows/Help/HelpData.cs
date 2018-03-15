// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.ComponentModel;
using System.Reflection;

namespace WAYWF.UI
{
	sealed class HelpData : INotifyPropertyChanged
	{
		public Version Version => _assembly.GetName().Version;
		public string Copyright => GetAttribute<AssemblyCopyrightAttribute>().Copyright;

		#region INotifyPropertyChanged Members

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add { }
			remove { }
		}

		#endregion

		T GetAttribute<T>()
			where T : Attribute
		{
			return (T)Attribute.GetCustomAttribute(_assembly, typeof(T));
		}

		Assembly _assembly = typeof(HelpData).Assembly;
	}
}
