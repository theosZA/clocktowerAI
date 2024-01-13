namespace Clocktower.EventScripts
{
    internal class EventNode : IEventScriptNode
    {
        public string Name { get; private set; }

        public EventNode(string name)
        {
            Name = name;
        }
    }
}
