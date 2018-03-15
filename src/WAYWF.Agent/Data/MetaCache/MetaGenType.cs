// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.ObjectModel;
using System.Diagnostics;
using WAYWF.Agent.CorDebugApi;

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

		public override void Apply(IMetaTypeVisitor visitor) => visitor.Visit(this);

		public override bool TryGetValue(ICorDebugValue value, out object result)
		{
			return BaseType.TryGetValue(value, TypeArgs, out result);
		}
	}
}
