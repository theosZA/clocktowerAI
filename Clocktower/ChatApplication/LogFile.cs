using OpenAi;

namespace ChatApplication
{
    internal class LogFile
    {
        public LogFile(string fileName)
        {
            var lines = File.ReadAllLines(fileName);
            IList<ChatMessage>? currentList = null;
            foreach (var line in lines)
            {
                AddLineFromLog(line, ref currentList);
            }
        }

        public IEnumerable<ChatMessage> BuildChatHistoryFromLatest()
        {
            if (log.Count == 0)
            {
                return Enumerable.Empty<ChatMessage>();
            }
            var (request, response) = log[^1];
            return request.Concat(response);
        }

        private void AddLineFromLog(string line, ref IList<ChatMessage>? currentList)
        {
            // The log file format is:
            // >> Request|Response NNN
            // [Role] Message
            // [Role] Message
            // 
            // Messages may be split over several lines.

            if (line.StartsWith(">> Request"))
            {
                currentList = StartNewRequest();
            }
            else if (line.StartsWith(">> Response"))
            {
                currentList = StartNewResponse();
            }
            else
            {
                currentList ??= StartNewRequest();  // To support simple handmade files, if we haven't start a request or response yet, we will consider this the first request.
                AddLineToCurrentList(currentList, line);
            }
        }

        private IList<ChatMessage> StartNewRequest()
        {
            log.Add((new(), new()));
            return log[^1].request;
        }

        private IList<ChatMessage> StartNewResponse()
        {
            if (log.Count == 0)
            {
                log.Add((new(), new()));
            }
            return log[^1].response;
        }

        private static void AddLineToCurrentList(IList<ChatMessage> current, string line)
        {
            if (line.StartsWith('['))
            {
                current.Add(BuildChatMessageFromLine(line));
            }
            else if (current.Count > 0)
            {
                current[^1].Message += Environment.NewLine + line;
            }
            // Else we don't have a role for the current line, so we ignore it.
        }

        private static ChatMessage BuildChatMessageFromLine(string line)
        {
            return new ChatMessage
            {
                Role = GetRole(line.TextBetween('[', ']')),
                Message = line.TextAfter("] ")
            };
        }

        private static Role GetRole(string roleText)
        {
            return (Role)Enum.Parse(typeof(Role), roleText);
        }

        private readonly List<(List<ChatMessage> request, List<ChatMessage> response)> log = new();
    }
}
