// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.ObjectModel;

namespace WAYWF.Agent
{
	static class EmptyCollections<T>
	{
		public static readonly T[] EmptyArray = new T[0];
		public static readonly ReadOnlyCollection<T> EmptyReadOnlyCollection = new ReadOnlyCollection<T>(EmptyArray);
	}
}
