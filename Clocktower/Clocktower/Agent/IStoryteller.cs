namespace Clocktower.Agent
{
    internal interface IStoryteller
    {
        public void AssignCharacter(string name, Character character, Alignment alignment);
        public void AssignCharacter(string name, Character realCharacter, Alignment realAlignment,
                                                 Character believedCharacter, Alignment believedAlignment);
    }
}
