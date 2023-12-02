namespace Clocktower.Agent
{
    internal class HumanStoryteller : IStoryteller
    {
        public HumanStoryteller(StorytellerForm form)
        {
            this.form = form;
        }

        public void AssignCharacter(string name, Character character, Alignment alignment)
        {
            form.AssignCharacter(name, character, alignment);
        }

        public void AssignCharacter(string name, Character realCharacter, Alignment realAlignment,
                                                 Character believedCharacter, Alignment believedAlignment)
        {
            form.AssignCharacter(name, realCharacter, realAlignment, believedCharacter, believedAlignment);
        }

        private StorytellerForm form;
    }
}
