// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WAYWF.Agent.CorDebugApi;
using WAYWF.Agent.Data;
using WAYWF.Agent.MetaCache;
using WAYWF.Agent.Source;

namespace WAYWF.Agent.PendingTasks
{
	sealed class PendingTaskFactory
	{
		public PendingTaskFactory(MetaDataCache cache, RuntimeValueFactory objects, SourceProvider sourceProvider)
		{
			_descriptorFactory = new StateMachineDescriptorFactory(cache);
			_mdCache = cache;
			_objects = objects;
			_sourceProvider = sourceProvider;
		}

		public PendingStateMachineTask[] ExtractPendingTasks(ICorDebugProcess5 process)
		{
			if (!process.AreGCStructuresValid())
			{
				return Array.Empty<PendingStateMachineTask>();
			}

			var e = process.EnumerateHeap();
			var result = new List<PendingStateMachineTask>();

			while (e.Next(1, out var obj))
			{
				if (TryGetPendingStateMachineTask(process, ref obj, out var task))
				{
					result.Add(task);
				}
			}

			return result.ToArray();
		}

		bool TryGetPendingStateMachineTask(ICorDebugProcess5 process, ref COR_HEAPOBJECT obj, out PendingStateMachineTask result)
		{
			var descriptor = GetStateMachineType(process, obj.type);

			if (descriptor == null)
			{
				result = null;
				return false;
			}

			var value = process.GetObject(obj.address);
			var objectValue = GetObjectValue(value);

			RuntimeSimpleValue stateValue = null;
			RuntimeValue thisValue = null;
			RuntimeValue taskValue = null;
			var parameterValues = new RuntimeValue[descriptor.ParamFields.Count];
			var localValues = new RuntimeValue[descriptor.LocalFields.Count];
			SourceAsyncState state = null;

			if (objectValue != null)
			{
				stateValue = descriptor.StateField == null ? null : GetValue(objectValue, descriptor.StateField.FieldToken) as RuntimeSimpleValue;
				thisValue = descriptor.ThisField == null ? null : GetValue(objectValue, descriptor.ThisField.FieldToken);

				if (descriptor.TaskFieldSequence.Count > 0)
				{
					taskValue = GetIndirectValue(objectValue, descriptor.TaskFieldSequence);
				}

				for (var i = 0; i < parameterValues.Length; i++)
				{
					var field = descriptor.ParamFields[i];
					parameterValues[i] = field == null ? null : GetValue(objectValue, field.FieldToken);
				}

				for (var i = 0; i < localValues.Length; i++)
				{
					var field = descriptor.LocalFields[i];
					localValues[i] = field == null ? null : GetValue(objectValue, field.Token);
				}

				if (stateValue != null && stateValue.Value is int && !descriptor.MoveNextMethod.IsNil)
				{
					var cl = value.GetClass();
					var module = cl.GetModule();
					state = _sourceProvider.GetAsyncSourceRef(module, descriptor.MoveNextMethod, (int)stateValue.Value);
				}
			}

			result = new PendingStateMachineTask(descriptor, GetTypeArgs(process, obj.type), stateValue, thisValue, taskValue, parameterValues, localValues, state);
			return true;
		}

		RuntimeValue GetIndirectValue(ICorDebugObjectValue objectValue, ReadOnlyCollection<MetaDataToken> taskSequence)
		{
			for (var i = 0; i < taskSequence.Count; i++)
			{
				var value = objectValue.GetFieldValue(taskSequence[i]);

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

				objectValue = value as ICorDebugObjectValue;

				if (objectValue == null)
				{
					return null;
				}
			}

			return _objects.GetValue(objectValue);
		}

		RuntimeValue GetValue(ICorDebugObjectValue value, MetaDataToken token)
		{
			var fieldValue = value.GetFieldValue(token);

			if (fieldValue == null)
			{
				return null;
			}

			return _objects.GetValue(fieldValue);
		}

		static ICorDebugObjectValue GetObjectValue(ICorDebugValue value)
		{
			if (value is ICorDebugBoxValue b)
			{
				value = b.GetObject();
			}

			return value as ICorDebugObjectValue;
		}

		ReadOnlyCollection<MetaTypeBase> GetTypeArgs(ICorDebugProcess5 process, COR_TYPEID typeID)
		{
			var type = process.GetTypeForTypeID(typeID);
			var e = type.EnumerateTypeParameters();
			var typeArgs = _mdCache.GetTypes(e).ToArray();
			return typeArgs.MakeReadOnly();
		}

		StateMachineDescriptor GetStateMachineType(ICorDebugProcess5 process, COR_TYPEID typeID)
		{
			if (!_cache.TryGetValue(typeID, out var result))
			{
				var type = process.GetTypeForTypeID(typeID);

				if (IsStateMachine(type))
				{
					result = _descriptorFactory.GetDescriptor(type.GetClass());
				}
				else
				{
					result = null;
				}

				_cache.Add(typeID, result);
			}

			return result;
		}

		static bool IsStateMachine(ICorDebugType type)
		{
			switch (type.GetType())
			{
				case CorElementType.ELEMENT_TYPE_VALUETYPE:
				case CorElementType.ELEMENT_TYPE_CLASS:
					break;

				default: return false;
			}

			return IsStateMachine(type.GetClass());
		}

		static bool IsStateMachine(ICorDebugClass cl)
		{
			return IsStateMachine(cl.GetModule(), cl.GetToken());
		}

		static bool IsStateMachine(ICorDebugModule module, MetaDataToken token)
		{
			if (module.IsDynamic())
			{
				return false;
			}

			var e = IntPtr.Zero;
			var import = module.GetMetaDataImport();

			try
			{
				while (import.EnumInterfaceImpls(ref e, token, out var iiImpl))
				{
					var iface = import.GetInterfaceImplProps(iiImpl, IntPtr.Zero);

					if (module.IsType(iface, "mscorlib", "System.Runtime.CompilerServices.IAsyncStateMachine"))
					{
						return true;
					}
				}
			}
			finally
			{
				if (e != IntPtr.Zero)
				{
					import.CloseEnum(e);
				}
			}

			return false;
		}

		readonly Dictionary<COR_TYPEID, StateMachineDescriptor> _cache = new Dictionary<COR_TYPEID, StateMachineDescriptor>();
		readonly StateMachineDescriptorFactory _descriptorFactory;
		readonly MetaDataCache _mdCache;
		readonly RuntimeValueFactory _objects;
		readonly SourceProvider _sourceProvider;
	}
}
