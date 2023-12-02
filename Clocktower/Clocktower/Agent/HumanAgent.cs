using Clocktower.Game;

namespace Clocktower.Agent
{
    internal class HumanAgent : IAgent
    {
        public HumanAgent(HumanAgentForm form)
        {
            this.form = form;
        }

        public void AssignCharacter(Character character, Alignment alignment)
        {
            form.AssignCharacter(character, alignment);
        }

        private HumanAgentForm form;
    }
}
