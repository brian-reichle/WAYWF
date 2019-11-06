// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace WAYWF.Agent.Data
{
	public sealed class MetaArrayType : MetaTypeBase
	{
		public MetaArrayType(MetaTypeBase elementType, int rank)
		{
			ElementType = elementType;
			Rank = rank;
		}

		public MetaTypeBase ElementType { get; }
		public int Rank { get; }

		public override void Apply(IMetaTypeVisitor visitor) => visitor.VisitArray(this);
		public override TResult Apply<TArg, TResult>(IMetaTypeVisitor<TArg, TResult> visitor, TArg arg) => visitor.VisitArray(this, arg);
	}
}
