// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using WAYWF.Agent.Data;

namespace WAYWF.Agent.Core
{
	interface ISignatureContext
	{
		MetaType GetType(MetaDataToken token);
		string GetParamName(int index);
		string GetLocalName(int index);
	}
}
