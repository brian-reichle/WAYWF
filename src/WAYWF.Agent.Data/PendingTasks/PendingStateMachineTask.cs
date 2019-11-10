// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Immutable;

namespace WAYWF.Agent.Data
{
	public sealed class PendingStateMachineTask : IMetaGenericContext
	{
		public PendingStateMachineTask(
			StateMachineDescriptor descriptor,
			ImmutableArray<MetaTypeBase> typeArgs,
			RuntimeSimpleValue stateValue,
			RuntimeValue thisValue,
			RuntimeValue taskValue,
			ImmutableArray<RuntimeValue> parameterValues,
			ImmutableArray<RuntimeValue> localValues,
			SourceAsyncState state)
		{
			if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));
			if (typeArgs == null) throw new ArgumentNullException(nameof(typeArgs));

			Descriptor = descriptor;
			TypeArgs = typeArgs;
			StateValue = stateValue;
			ThisValue = thisValue;
			TaskValue = taskValue;
			ParameterValues = parameterValues;
			LocalValues = localValues;
			State = state;
		}

		public StateMachineDescriptor Descriptor { get; }
		public ImmutableArray<MetaTypeBase> TypeArgs { get; }
		public RuntimeSimpleValue StateValue { get; }
		public RuntimeValue ThisValue { get; }
		public RuntimeValue TaskValue { get; }
		public ImmutableArray<RuntimeValue> ParameterValues { get; }
		public ImmutableArray<RuntimeValue> LocalValues { get; }
		public SourceAsyncState State { get; }

		#region IMetaGenericContext Members

		int IMetaGenericContext.StartOfMethodArgs => Descriptor.AsyncMethod.DeclaringType.TypeArgs;

		#endregion
	}
}
