// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using WAYWF.Agent.CorDebugApi;

namespace WAYWF.Agent.MetaCache
{
	sealed class MetaGCHandleType : MetaResolvedType
	{
		public const string TypeName = "System.Runtime.InteropServices.GCHandle";

		public MetaGCHandleType(MetaModule module, MetaDataToken token, MetaDataToken handleField)
			: base(module, token, null, TypeName, 0)
		{
			HandleField = handleField;
		}

		public MetaDataToken HandleField { get; }

		public override bool TryGetValue(ICorDebugValue value, out object result)
		{
			var objValue = (ICorDebugObjectValue)value;
			var handleObj = objValue.GetFieldValue(HandleField);

			if (handleObj == null)
			{
				result = null;
				return false;
			}

			result = ValueExtensions.GetIntPtr((ICorDebugGenericValue)handleObj).ToString();
			return true;
		}
	}
}
