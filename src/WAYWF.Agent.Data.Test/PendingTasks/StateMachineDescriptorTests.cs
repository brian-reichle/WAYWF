// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;
using NUnit.Framework;

namespace WAYWF.Agent.Data.Test;

[TestFixture]
public class StateMachineDescriptorTests
{
	[Test]
	public void Constructor_NullAsyncMethod_ThrowsArgumentNullException()
	{
		var stateMachineType = CreateDummyType("StateMachineType");

		Assert.That(
			() => new StateMachineDescriptor(
				asyncMethod: null,
				moveNextMethod: new MetaDataToken(0x06000002),
				stateMachineType: stateMachineType,
				stateField: null,
				thisField: null,
				taskFieldSequence: default,
				paramFields: default,
				localFields: default),
			Throws.ArgumentNullException.With.Property("ParamName").EqualTo("asyncMethod"));
	}

	[Test]
	public void Constructor_NullStateMachineType_ThrowsArgumentNullException()
	{
		var asyncMethod = CreateDummyMethod("MyAsyncMethod");

		Assert.That(
			() => new StateMachineDescriptor(
				asyncMethod: asyncMethod,
				moveNextMethod: new MetaDataToken(0x06000002),
				stateMachineType: null,
				stateField: null,
				thisField: null,
				taskFieldSequence: default,
				paramFields: default,
				localFields: default),
			Throws.ArgumentNullException.With.Property("ParamName").EqualTo("stateMachineType"));
	}

	[Test]
	public void Constructor_StoresPropertiesVerbatim()
	{
		var asyncMethod = CreateDummyMethod("MyAsyncMethod");
		var moveNextMethod = new MetaDataToken(0x06000002);
		var stateMachineType = CreateDummyType("StateMachineType");
		var stateField = new SMField(new MetaDataToken(0x04000001), "<>1__state");
		var thisField = new SMField(new MetaDataToken(0x04000002), "<>4__this");
		var taskFieldSequence = ImmutableArray.Create(new MetaDataToken(0x04000003));
		var paramFields = ImmutableArray.Create(new SMField(new MetaDataToken(0x04000004), "param1"));
		var localFields = ImmutableArray<MetaField>.Empty;

		var descriptor = new StateMachineDescriptor(
			asyncMethod,
			moveNextMethod,
			stateMachineType,
			stateField,
			thisField,
			taskFieldSequence,
			paramFields,
			localFields);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(descriptor.AsyncMethod, Is.SameAs(asyncMethod));
			Assert.That(descriptor.MoveNextMethod, Is.EqualTo(moveNextMethod));
			Assert.That(descriptor.StateMachineType, Is.SameAs(stateMachineType));
			Assert.That(descriptor.StateField, Is.SameAs(stateField));
			Assert.That(descriptor.ThisField, Is.SameAs(thisField));
			Assert.That(descriptor.TaskFieldSequence, Is.EqualTo(taskFieldSequence));
			Assert.That(descriptor.ParamFields, Is.EqualTo(paramFields));
			Assert.That(descriptor.LocalFields, Is.EqualTo(localFields));
		}
	}

	static MetaResolvedType CreateDummyType(string name)
	{
		return new MetaSimpleResolvedType(WellKnownMetaModules.SomeModule, new MetaDataToken(0x02000001), null, name, 0);
	}

	static MetaMethod CreateDummyMethod(string name)
	{
		var declaringType = CreateDummyType("DeclaringType");
		return new MetaMethod(new MetaDataToken(0x06000001), WellKnownMetaModules.SomeModule, declaringType, name, null, default);
	}
}
