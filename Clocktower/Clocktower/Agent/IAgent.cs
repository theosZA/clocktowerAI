using Clocktower.Game;
using Clocktower.Options;

namespace Clocktower.Agent
{
    public interface IAgent
    {
        void AssignCharacter(Character character, Alignment alignment);

        void Night(int nightNumber);
        void Day(int dayNumber);
        void PlayerDiedAtNight(Player newlyDeadPlayer);
        void PlayerIsExecuted(Player executedPlayer, bool playerDies);
        void DayEndsWithNoExecution();

        void MinionInformation(Player demon, IReadOnlyCollection<Player> fellowMinions);
        void DemonInformation(IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters);

        void NotifyGodfather(IReadOnlyCollection<Character> outsiders);
        void NotifyLibrarian(Player playerA, Player playerB, Character character);
        void NotifyInvestigator(Player playerA, Player playerB, Character character);
        void NotifySteward(Player goodPlayer);
        void NotifyEmpath(Player neighbourA, Player neighbourB, int evilCount);
        void NotifyRavenkeeper(Player target, Character character);

        void RequestChoiceFromImp(IReadOnlyCollection<IOption> options, Action<IOption> onChoice);
        void RequestChoiceFromRavenkeeper(IReadOnlyCollection<IOption> options, Action<IOption> onChoice);

        void GetNomination(IReadOnlyCollection<IOption> options, Action<IOption> onChoice);
        void AnnounceNomination(Player nominator, Player nominee);
        void GetVote(IReadOnlyCollection<IOption> options, Action<IOption> onChoice);
        void AnnounceVote(Player voter, Player nominee, bool votedToExecute);
        void AnnounceVoteResult(Player nominee, int voteCount, bool beatsCurrent, bool tiesCurrent);
    }
}
