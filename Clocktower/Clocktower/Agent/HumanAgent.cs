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

        public void Night(int nightNumber)
        {
            form.Night(nightNumber);
        }

        public void Day(int dayNumber)
        {
            form.Day(dayNumber);
        }

        public void MinionInformation(Player demon, IReadOnlyCollection<Player> fellowMinions)
        {
            form.MinionInformation(demon, fellowMinions);
        }

        public void DemonInformation(IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters)
        {
            form.DemonInformation(minions, notInPlayCharacters);
        }

        public void NotifyGodfather(IReadOnlyCollection<Character> outsiders)
        {
            form.NotifyGodfather(outsiders);
        }

        public void NotifyLibrarian(Player playerA, Player playerB, Character character)
        {
            form.NotifyLibrarian(playerA, playerB, character);
        }

        public void NotifySteward(Player goodPlayer)
        {
            form.NotifySteward(goodPlayer);
        }

        public void NotifyEmpath(Player neighbourA, Player neighbourB, int evilCount)
        {
            form.NotifyEmpath(neighbourA, neighbourB, evilCount);
        }

        private HumanAgentForm form;
    }
}
