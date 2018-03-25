// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.ObjectModel;
using WAYWF.Agent.MetaCache;

namespace WAYWF.Agent.PendingTasks
{
	sealed class StateMachineDescriptor
	{
		public StateMachineDescriptor(MetaMethod asyncMethod, MetaDataToken moveNextMethod, MetaResolvedType stateMachineType, SMField stateField, SMField thisField, SMField[] paramFields, MetaField[] localFields)
		{
			if (asyncMethod == null) throw new ArgumentNullException(nameof(asyncMethod));
			if (stateMachineType == null) throw new ArgumentNullException(nameof(stateMachineType));

			AsyncMethod = asyncMethod;
			MoveNextMethod = moveNextMethod;
			StateMachineType = stateMachineType;
			StateField = stateField;
			ThisField = thisField;
			ParamFields = paramFields.MakeReadOnly();
			LocalFields = localFields.MakeReadOnly();
		}

		public MetaMethod AsyncMethod { get; }
		public MetaDataToken MoveNextMethod { get; }
		public MetaResolvedType StateMachineType { get; }
		public SMField StateField { get; }
		public SMField ThisField { get; }
		public ReadOnlyCollection<SMField> ParamFields { get; }
		public ReadOnlyCollection<MetaField> LocalFields { get; }
	}
}
