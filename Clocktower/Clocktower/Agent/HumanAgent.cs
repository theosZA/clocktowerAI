using Clocktower.Game;
using Clocktower.Options;

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

        public void NotifyInvestigator(Player playerA, Player playerB, Character character)
        {
            form.NotifyInvestigator(playerA, playerB, character);
        }

        public void NotifySteward(Player goodPlayer)
        {
            form.NotifySteward(goodPlayer);
        }

        public void NotifyEmpath(Player neighbourA, Player neighbourB, int evilCount)
        {
            form.NotifyEmpath(neighbourA, neighbourB, evilCount);
        }

        public void NotifyRavenkeeper(Player target, Character character)
        {
            form.NotifyRavenkeeper(target, character);
        }

        public void RequestChoiceFromImp(IReadOnlyCollection<IOption> options, Action<IOption> onChoice)
        {
            form.RequestChoiceFromImp(options, onChoice);
        }

        public void RequestChoiceFromRavenkeeper(IReadOnlyCollection<IOption> options, Action<IOption> onChoice)
        {
            form.RequestChoiceFromRavenkeeper(options, onChoice);
        }

        public void GetNomination(IReadOnlyCollection<IOption> options, Action<IOption> onChoice)
        {
            form.GetNomination(options, onChoice);
        }

        public void GetVote(IReadOnlyCollection<IOption> options, Action<IOption> onChoice)
        {
            form.GetVote(options, onChoice);
        }

        private HumanAgentForm form;
    }
}
