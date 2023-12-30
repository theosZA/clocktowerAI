using OpenAi;

namespace Clocktower.Agent.RobotAgent
{
    public class FileChatLogger : IChatLogger
    {
        public FileChatLogger(string fileName)
        {
            streamWriter = new StreamWriter(fileName);
        }

        public void Log(string subChatName, Role role, string message)
        {
            streamWriter.WriteLine($"[{role}] {message}");
            streamWriter.Flush();
        }

        public void LogSummary(string subChatName, string summary)
        {
            streamWriter.WriteLine($"[Summary of {subChatName}] {summary}");
            streamWriter.Flush();
        }

        private readonly TextWriter streamWriter;
    }
}