using OpenAi;

namespace Clocktower.Agent.RobotAgent
{
    public delegate void ChatMessageHandler(Role role, string message);
    public delegate void DaySummaryHandler(int dayNumber, string summary);
    public delegate void TokenCountHandler(int promptTokens, int completionTokens, int totalTokens);
}
