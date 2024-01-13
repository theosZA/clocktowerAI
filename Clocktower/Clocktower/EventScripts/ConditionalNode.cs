namespace Clocktower.EventScripts
{
    internal class ConditionalNode : IEventScriptNode
    {
        public string Expression { get; private set; }

        public IEventScriptNode Child { get; private set; }

        public ConditionalNode(string expression, IEventScriptNode child)
        {
            Expression = expression;
            Child = child;
        }
    }
}
