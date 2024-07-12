namespace Clocktower.EventScripts
{
    internal class EventScript
    {
        public IReadOnlyCollection<IEventScriptNode> EventNodes { get; }

        public EventScript(string fileName)
        {
            // We cache the event scripts. Note that this doesn't really help for ordinary gameplay,
            // but can potentially speed up tests so we don't reload the event script files for every test.
            if (CachedScripts.TryGetValue(fileName, out var eventNodes))
            {
                EventNodes = eventNodes;
                return;
            }

            EventNodes = ParseEventScript(ReadEventScriptFromFile(fileName)).ToList();
            CachedScripts.Add(fileName, EventNodes);
        }

        private static List<string> ReadEventScriptFromFile(string fileName)
        {
            return File.ReadAllLines(fileName).Select(line => line.TextBefore("//").Trim())
                                              .Where(line => !string.IsNullOrEmpty(line))
                                              .ToList();
        }

        private static IEnumerable<IEventScriptNode> ParseEventScript(List<string> lines)
        {
            int currentLine = 0;
            while (currentLine < lines.Count)
            {
                yield return ParseNode(lines, ref currentLine);
            }
        }

        private static IEventScriptNode ParseNode(List<string> lines, ref int currentLine)
        {
            var line = lines[currentLine++];
            if (line == "{")
            {
                return ParseSequenceNode(lines, ref currentLine);
            }
            if (line.StartsWith("if ") && line.EndsWith(":"))
            {
                // The expression is between the "if " and the ":"
                var expression = line[3..^1].Trim();
                return new ConditionalNode(expression, ParseNode(lines, ref currentLine));
            }
            // Otherwise we have a regular event.
            return new EventNode(line);
        }

        private static IEventScriptNode ParseSequenceNode(List<string> lines, ref int currentLine)
        {
            var children = new List<IEventScriptNode>();
            while (lines[currentLine] != "}")
            {
                children.Add(ParseNode(lines, ref currentLine));
            }
            ++currentLine;  // skip over the '}'
            return new SequenceNode(children);
        }

        private static readonly Dictionary<string, IReadOnlyCollection<IEventScriptNode>> CachedScripts = new();
    }
}
