// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Immutable;
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test;

[TestFixture]
public class PendingStateMachineTaskTests
{
	[Test]
	public void Constructor_NullDescriptor_ThrowsArgumentNullException()
	{
		var typeArgs = ImmutableArray<MetaTypeBase>.Empty;

		Assert.That(
			() => new PendingStateMachineTask(
				descriptor: null,
				typeArgs: typeArgs,
				stateValue: null,
				thisValue: null,
				taskValue: null,
				parameterValues: default,
				localValues: default,
				state: null),
			Throws.ArgumentNullException.With.Property("ParamName").EqualTo("descriptor"));
	}

	[Test]
	public void Constructor_NullTypeArgs_ThrowsArgumentNullException()
	{
		var descriptor = CreateDummyDescriptor();

		Assert.That(
			() => new PendingStateMachineTask(
				descriptor: descriptor,
				typeArgs: default,
				stateValue: null,
				thisValue: null,
				taskValue: null,
				parameterValues: default,
				localValues: default,
				state: null),
			Throws.ArgumentNullException.With.Property("ParamName").EqualTo("typeArgs"));
	}

	[Test]
	public void Constructor_StoresPropertiesVerbatim()
	{
		var descriptor = CreateDummyDescriptor();
		var type = CreateDummyType("TArg");
		var typeArgs = ImmutableArray.Create<MetaTypeBase>(type);
		var stateValue = new RuntimeSimpleValue(Identity.NewSource().New(), type, 0);
		var thisValue = RuntimeNullValue.Instance;
		var taskValue = RuntimeNullValue.Instance;
		var parameterValues = ImmutableArray<RuntimeValue>.Empty;
		var localValues = ImmutableArray<RuntimeValue>.Empty;
		var state = new SourceAsyncState(12, null);

		var task = new PendingStateMachineTask(
			descriptor,
			typeArgs,
			stateValue,
			thisValue,
			taskValue,
			parameterValues,
			localValues,
			state);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(task.Descriptor, Is.SameAs(descriptor));
			Assert.That(task.TypeArgs, Is.EqualTo(typeArgs));
			Assert.That(task.StateValue, Is.SameAs(stateValue));
			Assert.That(task.ThisValue, Is.SameAs(thisValue));
			Assert.That(task.TaskValue, Is.SameAs(taskValue));
			Assert.That(task.ParameterValues, Is.EqualTo(parameterValues));
			Assert.That(task.LocalValues, Is.EqualTo(localValues));
			Assert.That(task.State, Is.SameAs(state));
		}
	}

	[Test]
	public void IMetaGenericContext_StartOfMethodArgs_ReturnsDeclaringTypeTypeArgs()
	{
		const int expectedTypeArgsCount = 3;
		var declaringType = new MetaSimpleResolvedType(
			WellKnownMetaModules.SomeModule,
			new MetaDataToken(0x02000001),
			null,
			"DeclaringType",
			expectedTypeArgsCount);

		var asyncMethod = new MetaMethod(
			new MetaDataToken(0x06000001),
			WellKnownMetaModules.SomeModule,
			declaringType,
			"AsyncMethod",
			null,
			default);

		var descriptor = new StateMachineDescriptor(
			asyncMethod,
			new MetaDataToken(0x06000002),
			declaringType,
			null,
			null,
			default,
			default,
			default);

		var task = new PendingStateMachineTask(
			descriptor,
			[],
			null,
			null,
			null,
			default,
			default,
			null);

		IMetaGenericContext genericContext = task;
		Assert.That(genericContext.StartOfMethodArgs, Is.EqualTo(expectedTypeArgsCount));
	}

	static MetaSimpleResolvedType CreateDummyType(string name)
	{
		return new MetaSimpleResolvedType(WellKnownMetaModules.SomeModule, new MetaDataToken(0x02000001), null, name, 0);
	}

	static StateMachineDescriptor CreateDummyDescriptor()
	{
		var declaringType = CreateDummyType("DeclaringType");
		var asyncMethod = new MetaMethod(new MetaDataToken(0x06000001), WellKnownMetaModules.SomeModule, declaringType, "AsyncMethod", null, default);
		return new StateMachineDescriptor(asyncMethod, new MetaDataToken(0x06000002), declaringType, null, null, default, default, default);
	}
}
