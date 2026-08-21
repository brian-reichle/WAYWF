// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace WAYWF.Agent.Data.Test;

public static class WellKnownMetaModules
{
	public static MetaAssembly SomeAssembly { get; } = new MetaAssembly(
		"some/path",
		"SomeAssembly",
		new Version(1, 0),
		null,
		null);

	public static MetaModule SomeModule { get; } = new MetaModule(
		SomeAssembly,
		Identity.NewSource().New(),
		"some/path/module",
		"SomeModule",
		false,
		false,
		Guid.NewGuid());
}
