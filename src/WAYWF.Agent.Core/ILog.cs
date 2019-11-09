namespace WAYWF.Agent.Core
{
	public interface ILog
	{
		void WriteLine(string message);
		void WriteFormattedLine(string message, params object[] args);
	}
}
