namespace OpenAi
{
    public interface IChatLogger
    {
        void Log(string subChatName, Role role, string message);

        void LogSummary(string subChatName, string summary);
    }
}