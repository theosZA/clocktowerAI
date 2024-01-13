namespace Clocktower.EventScripts
{
    internal class SequenceNode : IEventScriptNode
    {
        public IReadOnlyCollection<IEventScriptNode> Children { get; private set; }

        public SequenceNode(IReadOnlyCollection<IEventScriptNode> children)
        {
            Children = children;
        }
    }
}
