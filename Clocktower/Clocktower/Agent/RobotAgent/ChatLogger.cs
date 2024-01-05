using OpenAi;

namespace Clocktower.Agent.RobotAgent
{
    /// <summary>
    /// Logs individual messages added to a chat as well as the request-response pairs made to a chat AI.
    /// </summary>
    internal class ChatLogger
    {
        public ChatLogger(string chatName)
        {
            var timestamp = DateTime.UtcNow;
            messageLogStream = new StreamWriter($"messages-{chatName}-{timestamp:yyyyMMddTHHmmss}.log");
            requestLogStream = new StreamWriter($"requests-{chatName}-{timestamp:yyyyMMddTHHmmss}.log");
        }

        public void MessageAdded(Role role, string message)
        {
            messageLogStream.WriteLine($"[{role}] {message}");
            messageLogStream.Flush();
        }

        public void SubChatSummarized(string subChatName, string summary)
        {
            messageLogStream.WriteLine($"[Summary of {subChatName}] {summary}");
            messageLogStream.Flush();
        }

        public void AssistantRequest(IReadOnlyCollection<(Role role, string message)> messages, string response, int promptTokens, int completionTokens)
        {
            ++requestCounter;
            requestLogStream.WriteLine($">> Request {requestCounter} ({promptTokens} tokens)");
            foreach (var (role, message) in messages)
            {
                requestLogStream.WriteLine($"[{role}] {message}");
            }
            requestLogStream.WriteLine($">> Response {requestCounter} ({completionTokens} tokens)");
            requestLogStream.WriteLine($"[{Role.Assistant}] {response}");
            requestLogStream.Flush();
        }

        private readonly TextWriter messageLogStream;
        private readonly TextWriter requestLogStream;
        private int requestCounter = 0;
    }
}
