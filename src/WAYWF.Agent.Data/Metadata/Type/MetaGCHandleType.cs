// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace WAYWF.Agent.Data
{
	public sealed class MetaGCHandleType : MetaResolvedType
	{
		public const string TypeName = "System.Runtime.InteropServices.GCHandle";

		public MetaGCHandleType(MetaModule module, MetaDataToken token, MetaDataToken handleField)
			: base(module, token, null, TypeName, 0)
		{
			HandleField = handleField;
		}

		public MetaDataToken HandleField { get; }

		public override void Apply(IMetaTypeVisitor visitor) => visitor.VisitGCHandle(this);
		public override TResult Apply<TArg, TResult>(IMetaTypeVisitor<TArg, TResult> visitor, TArg arg) => visitor.VisitGCHandle(this, arg);
	}
}
