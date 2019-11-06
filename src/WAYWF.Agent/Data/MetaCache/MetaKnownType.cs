// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace WAYWF.Agent.MetaCache
{
	sealed partial class MetaKnownType : MetaResolvedType
	{
		MetaKnownType(MetaKnownTypeCode code, string fullName, int size)
			: base(null, MetaDataToken.Nil, null, fullName, 0)
		{
			Code = code;
			Size = size;
		}

		public static MetaKnownType FromFullName(string fullName)
		{
			_lookup.TryGetValue(fullName, out var result);
			return result;
		}

		public MetaKnownTypeCode Code { get; }
		public int Size { get; }

		public override void Apply(IMetaTypeVisitor visitor) => visitor.VisitKnownType(this);
		public override TResult Apply<TArg, TResult>(IMetaTypeVisitor<TArg, TResult> visitor, TArg arg) => visitor.VisitKnownType(this, arg);
	}
}
