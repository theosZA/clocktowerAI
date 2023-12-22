namespace Clocktower.OpenAiApi
{
    public interface IChatLogger
    {
        void Log(Role role, string message);

        void LogSummary(Phase phase, int dayNumber, string summary);
    }
}