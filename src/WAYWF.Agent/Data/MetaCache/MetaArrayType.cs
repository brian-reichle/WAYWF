// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using WAYWF.Agent.CorDebugApi;

namespace WAYWF.Agent.MetaCache
{
	sealed class MetaArrayType : MetaTypeBase
	{
		public MetaArrayType(MetaTypeBase elementType, int rank)
		{
			ElementType = elementType;
			Rank = rank;
		}

		public MetaTypeBase ElementType { get; }
		public int Rank { get; }
		public override void Apply(IMetaTypeVisitor visitor) => visitor.Visit(this);

		public override bool TryGetValue(ICorDebugValue value, out object result)
		{
			if (value is ICorDebugArrayValue arr)
			{
				var rank = arr.GetRank();
				var dims = new int[rank];
				arr.GetDimensions(rank, dims);

				var formatter = new MetaFormatter();
				formatter.Write(this, dims);

				result = formatter.ToString();
				return true;
			}
			else
			{
				result = null;
				return false;
			}
		}
	}
}
