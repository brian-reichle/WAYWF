// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace WAYWF.Agent.Data
{
	public sealed class MetaPointerType : MetaTypeBase
	{
		public MetaPointerType(MetaTypeBase elementType)
		{
			ElementType = elementType;
		}

		public MetaTypeBase ElementType { get; }

		public override void Apply(IMetaTypeVisitor visitor) => visitor.VisitPointer(this);
		public override TResult Apply<TArg, TResult>(IMetaTypeVisitor<TArg, TResult> visitor, TArg arg) => visitor.VisitPointer(this, arg);
	}
}
