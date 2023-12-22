namespace Clocktower.OpenAiApi
{
    public class FileChatLogger : IChatLogger
    {
        public FileChatLogger(string fileName)
        {
            streamWriter = new StreamWriter(fileName);
        }

        public void Log(Role role, string message)
        {
            streamWriter.WriteLine($"[{role}] {message}");
            streamWriter.Flush();
        }

        public void LogSummary(Phase phase, int dayNumber, string summary)
        {
            streamWriter.WriteLine($"[Summary of {phase} {dayNumber}] {summary}");
            streamWriter.Flush();
        }

        private readonly TextWriter streamWriter;
    }
}