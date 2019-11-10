// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Immutable;

namespace WAYWF.Agent.Data
{
	public sealed class StateMachineDescriptor
	{
		public StateMachineDescriptor(MetaMethod asyncMethod, MetaDataToken moveNextMethod, MetaResolvedType stateMachineType, SMField stateField, SMField thisField, ImmutableArray<MetaDataToken> taskFieldSequence, ImmutableArray<SMField> paramFields, ImmutableArray<MetaField> localFields)
		{
			if (asyncMethod == null) throw new ArgumentNullException(nameof(asyncMethod));
			if (stateMachineType == null) throw new ArgumentNullException(nameof(stateMachineType));

			AsyncMethod = asyncMethod;
			MoveNextMethod = moveNextMethod;
			StateMachineType = stateMachineType;
			StateField = stateField;
			ThisField = thisField;
			TaskFieldSequence = taskFieldSequence;
			ParamFields = paramFields;
			LocalFields = localFields;
		}

		public MetaMethod AsyncMethod { get; }
		public MetaDataToken MoveNextMethod { get; }
		public MetaResolvedType StateMachineType { get; }
		public SMField StateField { get; }
		public SMField ThisField { get; }
		public ImmutableArray<MetaDataToken> TaskFieldSequence { get; }
		public ImmutableArray<SMField> ParamFields { get; }
		public ImmutableArray<MetaField> LocalFields { get; }
	}
}
