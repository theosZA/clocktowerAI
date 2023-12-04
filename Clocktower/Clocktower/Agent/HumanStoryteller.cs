using Clocktower.Game;

namespace Clocktower.Agent
{
    internal class HumanStoryteller : IStoryteller
    {
        public HumanStoryteller(StorytellerForm form)
        {
            this.form = form;
        }

        public void AssignCharacter(Player player)
        {
            form.AssignCharacter(player);
        }

        public void Night(int nightNumber)
        {
            form.Night(nightNumber);
        }

        public void Day(int dayNumber)
        {
            form.Day(dayNumber);
        }

        public void PlayerDiedAtNight(Player newlyDeadPlayer)
        {
            form.PlayerDiedAtNight(newlyDeadPlayer);
        }

        public void PlayerIsExecuted(Player executedPlayer, bool playerDies)
        {
            form.PlayerIsExecuted(executedPlayer, playerDies);
        }

        public void DayEndsWithNoExecution()
        {
            form.DayEndsWithNoExecution();
        }

        public void MinionInformation(Player minion, Player demon, IReadOnlyCollection<Player> fellowMinions)
        {
            form.MinionInformation(minion, demon, fellowMinions);
        }

        public void DemonInformation(Player demon, IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters)
        {
            form.DemonInformation(demon, minions, notInPlayCharacters);
        }

        public void NotifyGodfather(Player godfather, IReadOnlyCollection<Character> outsiders)
        {
            form.NotifyGodfather(godfather, outsiders);
        }

        public void NotifyLibrarian(Player librarian, Player playerA, Player playerB, Character character)
        {
            form.NotifyLibrarian(librarian, playerA, playerB, character);
        }

        public void NotifyInvestigator(Player investigator, Player playerA, Player playerB, Character character)
        {
            form.NotifyInvestigator(investigator, playerA, playerB, character);
        }

        public void NotifySteward(Player steward, Player goodPlayer)
        {
            form.NotifySteward(steward, goodPlayer);
        }

        public void NotifyEmpath(Player empath, Player neighbourA, Player neighbourB, int evilCount)
        {
            form.NotifyEmpath(empath, neighbourA, neighbourB, evilCount);
        }

        public void ChoiceFromImp(Player imp, Player target)
        {
            form.ChoiceFromImp(imp, target);
        }

        public void ChoiceFromRavenkeeper(Player ravenkeeper, Player target, Character character)
        {
            form.ChoiceFromRavenkeeper(ravenkeeper, target, character);
        }

        public void AnnounceNomination(Player nominator, Player nominee)
        {
            form.AnnounceNomination(nominator, nominee);
        }

        public void AnnounceVote(Player voter, Player nominee, bool votedToExecute)
        {
            form.AnnounceVote(voter, nominee, votedToExecute);
        }

        public void AnnounceVoteResult(Player nominee, int voteCount, bool beatsCurrent, bool tiesCurrent)
        {
            form.AnnounceVoteResult(nominee, voteCount, beatsCurrent, tiesCurrent);
        }

        private StorytellerForm form;
    }
}
