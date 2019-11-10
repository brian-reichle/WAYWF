// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Immutable;
using System.Diagnostics;

namespace WAYWF.Agent.Data
{
	[DebuggerDisplay("Frame: {Method.Name,nq} +{ILOffset}")]
	public sealed class RuntimeILFrame : RuntimeFrame, IMetaGenericContext
	{
		public RuntimeILFrame(
			MetaMethod method,
			int ilOffset,
			RuntimeILMapping ilMapping,
			SourceRef source,
			RuntimeValue @this,
			ImmutableArray<MetaTypeBase> typeArgs,
			ImmutableArray<RuntimeValue> arguments,
			ImmutableArray<RuntimeValue> locals,
			ImmutableArray<string> localNames)
		{
			Method = method ?? throw new ArgumentNullException(nameof(method));
			ILOffset = ilOffset;
			ILMapping = ilMapping;
			Source = source;
			This = @this;
			TypeArgs = typeArgs;
			Arguments = arguments;
			Locals = locals;
			LocalNames = localNames;

			if (method.DeclaringType != null)
			{
				StartOfMethodArgs = method.DeclaringType.TypeArgs;
			}
		}

		public MetaMethod Method { get; }
		public int StartOfMethodArgs { get; }
		public int ILOffset { get; }
		public RuntimeILMapping ILMapping { get; }
		public SourceRef Source { get; }
		public RuntimeValue This { get; }

		public double? Duration { get; set; }

		public ImmutableArray<MetaTypeBase> TypeArgs { get; }
		public ImmutableArray<RuntimeValue> Arguments { get; }
		public ImmutableArray<RuntimeValue> Locals { get; }
		public ImmutableArray<string> LocalNames { get; }

		public override void Apply(IRuntimeFrameVisitor visitor) => visitor.Visit(this);
	}
}
