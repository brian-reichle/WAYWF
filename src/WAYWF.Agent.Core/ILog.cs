// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace WAYWF.Agent.Core;

public interface ILog
{
	void WriteLine(string message);
	void WriteFormattedLine(string message, params object[] args);
}
