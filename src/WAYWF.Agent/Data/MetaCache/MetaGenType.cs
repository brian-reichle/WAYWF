// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace WAYWF.Agent.MetaCache
{
	[DebuggerDisplay("GenType: {BaseType.Name,nq}")]
	sealed class MetaGenType : MetaTypeBase
	{
		public MetaGenType(MetaType baseType, MetaTypeBase[] typeArgs)
		{
			BaseType = baseType;
			TypeArgs = typeArgs.MakeReadOnly();
		}

		public MetaType BaseType { get; }
		public ReadOnlyCollection<MetaTypeBase> TypeArgs { get; }

		public override void Apply(IMetaTypeVisitor visitor) => visitor.VisitGen(this);
		public override TResult Apply<TArg, TResult>(IMetaTypeVisitor<TArg, TResult> visitor, TArg arg) => visitor.VisitGen(this, arg);
	}
}
