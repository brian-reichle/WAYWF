// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace WAYWF.Agent
{
	[DebuggerDisplay("ID = {_id}")]
	sealed class Identity
	{
		public static IIdentitySource NewSource() => new IdentitySource();

		Identity(IdentitySource source)
		{
			_source = source;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public int ID
		{
			get
			{
				if (_id == 0)
				{
					Interlocked.CompareExchange(ref _id, _source.NextID(), 0);
				}

				return _id;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		int _id;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly IdentitySource _source;

		public override string ToString() => ID.ToString(CultureInfo.InvariantCulture);

		sealed class IdentitySource : IIdentitySource
		{
			public Identity New() => new Identity(this);

			public int NextID()
			{
				var result = Interlocked.Increment(ref _nextID);

				if (result < 0)
				{
					_nextID = int.MaxValue;
					throw new InvalidOperationException("identity overflow.");
				}

				return result;
			}

			int _nextID;
		}
	}
}
