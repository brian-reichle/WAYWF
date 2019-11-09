// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.ObjectModel;
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
			MetaTypeBase[] typeArgs,
			RuntimeValue[] arguments,
			RuntimeValue[] locals,
			string[] localNames)
		{
			Method = method ?? throw new ArgumentNullException(nameof(method));
			ILOffset = ilOffset;
			ILMapping = ilMapping;
			Source = source;
			This = @this;
			TypeArgs = typeArgs.MakeReadOnly();
			Arguments = arguments.MakeReadOnly();
			Locals = locals.MakeReadOnly();
			LocalNames = localNames.MakeReadOnly();

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

		public ReadOnlyCollection<MetaTypeBase> TypeArgs { get; }
		public ReadOnlyCollection<RuntimeValue> Arguments { get; }
		public ReadOnlyCollection<RuntimeValue> Locals { get; }
		public ReadOnlyCollection<string> LocalNames { get; }

		public override void Apply(IRuntimeFrameVisitor visitor) => visitor.Visit(this);
	}
}
