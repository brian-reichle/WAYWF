// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace WAYWF.Agent.Data.Test;

abstract class DummyBaseValueVisitor : IRuntimeValueVisitor
{
	public virtual void Visit(RuntimeNullValue value) => Visit(value, typeof(RuntimeNullValue));
	public virtual void Visit(RuntimeSimpleValue value) => Visit(value, typeof(RuntimeSimpleValue));
	public virtual void Visit(RuntimeRcwValue value) => Visit(value, typeof(RuntimeRcwValue));
	public virtual void Visit(RuntimePointerValue value) => Visit(value, typeof(RuntimePointerValue));

	protected virtual void Visit(RuntimeValue value, Type identifiedType) => throw new InvalidOperationException($"Did not expect the {identifiedType.Name} overload to be called.");
}
