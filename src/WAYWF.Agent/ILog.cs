namespace WAYWF.Agent
{
	interface ILog
	{
		void WriteLine(string message);
		void WriteFormattedLine(string message, params object[] args);
	}
}
