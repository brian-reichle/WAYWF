// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace WAYWF.Agent.Data
{
	public sealed class SourceRef
	{
		public SourceRef(SourceDocument document, int fromLine, int toLine, int fromColumn, int toColumn)
		{
			Document = document;
			FromLine = fromLine;
			ToLine = toLine;
			FromColumn = fromColumn;
			ToColumn = toColumn;
		}

		public SourceDocument Document { get; }
		public int FromLine { get; }
		public int ToLine { get; }
		public int FromColumn { get; }
		public int ToColumn { get; }
	}
}
