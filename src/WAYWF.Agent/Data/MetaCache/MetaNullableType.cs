// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.ObjectModel;
using WAYWF.Agent.CorDebugApi;

namespace WAYWF.Agent.MetaCache
{
	sealed class MetaNullableType : MetaResolvedType
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

		public override bool TryGetValue(ICorDebugValue value, ReadOnlyCollection<MetaTypeBase> typeArgs, out object result)
		{
			if (typeArgs.Count != 1)
			{
				throw new InvalidMetaDataException("Nullable should have exactly 1 type arg");
			}

			var innerType = typeArgs[0];
			var objValue = (ICorDebugObjectValue)value;
			var hasValueValue = objValue.GetFieldValue(HasValueToken);

			if (HasValueToken == null)
			{
				result = null;
				return false;
			}

			if (!ValueExtensions.GetBoolean((ICorDebugGenericValue)hasValueValue))
			{
				result = null;
				return true;
			}

			var valueValue = objValue.GetFieldValue(ValueToken);

			if (valueValue == null)
			{
				result = null;
				return false;
			}

			return innerType.TryGetValue(valueValue, out result);
		}
	}
}
