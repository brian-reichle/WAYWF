// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Generic;
using WAYWF.Agent.Core.CorDebugApi;
using WAYWF.Agent.Data;

namespace WAYWF.Agent.Core
{
	sealed class RuntimeValueFactory
	{
		public RuntimeValueFactory(MetaDataCache cache, RuntimeNativeInterfaceFactory mapper)
		{
			_cache = cache;
			_mapper = mapper;
			_objects = new Dictionary<CORDB_ADDRESS, RuntimeValue>();
			_valueIdentites = Identity.NewSource();
		}

		public RuntimeValue GetValue(ICorDebugValue value)
		{
			switch (value.GetType())
			{
				case CorElementType.ELEMENT_TYPE_PTR: return GetPointerValue(value);
			}

			if (value is ICorDebugReferenceValue reference)
			{
				if (reference.IsNull())
				{
					return RuntimeNullValue.Instance;
				}

				value = reference.Dereference();

				if (value == null)
				{
					return RuntimeNullValue.Instance;
				}
			}
			else if (!(value is ICorDebugHeapValue))
			{
				return CreateValue(value);
			}

			return GetCachedValue(value);
		}

		public IEnumerable<RuntimeValue> Values => _objects.Values;

		RuntimeValue GetCachedValue(ICorDebugValue value)
		{
			var address = value.GetAddress();

			if (!_objects.TryGetValue(address, out var result))
			{
				_objects.Add(address, result = CreateValue(value));
			}
			else
			{
				result.ReferenceCount++;
			}

			return result;
		}

		RuntimeValue CreateValue(ICorDebugValue value)
		{
			if (value is ICorDebugBoxValue box)
			{
				value = box.GetObject();
			}

			if (value is ICorDebugComObjectValue rcw)
			{
				return CreateRCWValue(rcw);
			}
			else
			{
				return CreateSimpleValue(value);
			}
		}

		RuntimeValue CreateSimpleValue(ICorDebugValue value)
		{
			var type = GetValueType((ICorDebugValue2)value);

			if (!type.TryGetValue(value, out var pvalue) || pvalue != null)
			{
				return new RuntimeSimpleValue(_valueIdentites.New(), type, pvalue);
			}
			else
			{
				return RuntimeNullValue.Instance;
			}
		}

		RuntimeValue CreateRCWValue(ICorDebugComObjectValue rcw)
		{
			return new RuntimeRcwValue(
				_valueIdentites.New(),
				GetValueType((ICorDebugValue2)rcw),
				_cache.GetTypes(rcw.GetCachedInterfaceTypes()),
				_mapper.GetInterfaces(rcw.GetInterfacePointers()));
		}

		RuntimeValue GetPointerValue(ICorDebugValue value)
		{
			var reference = (ICorDebugReferenceValue)value;

			if (reference.IsNull())
			{
				return RuntimeNullValue.Instance;
			}

			var inner = reference.Dereference();

			return new RuntimePointerValue(
				GetValueType((ICorDebugValue2)value),
				reference.GetValue(),
				inner == null ? null : GetValue(inner));
		}

		MetaTypeBase GetValueType(ICorDebugValue2 value2)
		{
			return _cache.GetType(value2.GetExactType());
		}

		readonly MetaDataCache _cache;
		readonly RuntimeNativeInterfaceFactory _mapper;
		readonly Dictionary<CORDB_ADDRESS, RuntimeValue> _objects;
		readonly IIdentitySource _valueIdentites;
	}
}
