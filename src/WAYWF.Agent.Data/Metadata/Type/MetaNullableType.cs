// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace WAYWF.Agent.Data
{
	public sealed class MetaNullableType : MetaResolvedType
	{
		public const string TypeName = "System.Nullable`1";

		public MetaNullableType(MetaModule module, MetaDataToken token, MetaDataToken hasValueToken, MetaDataToken valueToken)
			: base(module, token, null, TypeName, 1)
		{
			HasValueToken = hasValueToken;
			ValueToken = valueToken;
		}

		public MetaDataToken HasValueToken { get; }
		public MetaDataToken ValueToken { get; }

		public override void Apply(IMetaTypeVisitor visitor) => visitor.VisitNullable(this);
		public override TResult Apply<TArg, TResult>(IMetaTypeVisitor<TArg, TResult> visitor, TArg arg) => visitor.VisitNullable(this, arg);
	}
}
