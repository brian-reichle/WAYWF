// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace WAYWF.Agent.Data
{
	interface IRuntimeValueVisitor
	{
		void Visit(RuntimeNullValue value);
		void Visit(RuntimeSimpleValue value);
		void Visit(RuntimeRcwValue value);
		void Visit(RuntimePointerValue value);
	}
}
