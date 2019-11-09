// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.ObjectModel;

namespace WAYWF.Agent.Data
{
	static class CollectionExtensions
	{
		public static ReadOnlyCollection<T> MakeReadOnly<T>(this T[] array)
		{
			return array == null || array.Length == 0 ? EmptyCollections<T>.EmptyReadOnlyCollection : new ReadOnlyCollection<T>(array);
		}

		static class EmptyCollections<T>
		{
			public static readonly ReadOnlyCollection<T> EmptyReadOnlyCollection = new ReadOnlyCollection<T>(Array.Empty<T>());
		}
	}
}
