using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using WAYWF.Agent.Core;

namespace WAYWF.Agent
{
	sealed class ConsoleLog : ILog
	{
		public void WriteLine(string message)
		{
			BeginMessage();
			_builder.Append(message);
			EndMessage();
		}

		public void WriteFormattedLine(string message, object[] args)
		{
			BeginMessage();
			_builder.AppendFormat(CultureInfo.InvariantCulture, message, args);
			EndMessage();
		}

		void BeginMessage()
		{
			_builder.Length = 0;
			_builder.AppendFormat(CultureInfo.InvariantCulture, "{0:0.000}: ", _sw.Elapsed.TotalSeconds);

			if (!_sw.IsRunning)
			{
				_sw.Start();
			}
		}

		void EndMessage()
		{
			Console.Error.WriteLine(_builder.ToString());
		}

		readonly StringBuilder _builder = new StringBuilder();
		readonly Stopwatch _sw = new Stopwatch();
	}
}
