using Clocktower.Game;
using Clocktower.Selection;
using Clocktower.Setup;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class SelectionOfKazaliMinions : IGameEvent
    {
        public SelectionOfKazaliMinions(IStoryteller storyteller, Grimoire grimoire, IReadOnlyCollection<Character> script)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.script = script;
        }

        public async Task RunEvent()
        {
            foreach (var kazali in grimoire.PlayersForWhomWeShouldRunAbility(Character.Kazali))
            {
                int minionCount = CharacterTypeDistribution.GetBaseMinionCount(grimoire.Players.Count);
                var possiblePlayers = grimoire.Players.Where(player => player != kazali).ToList();
                var minionCharacters = script.Where(character => character.CharacterType() == CharacterType.Minion).ToList();

                var kazaliMinionSelection = new KazaliMinionsSelection(minionCount, possiblePlayers, minionCharacters);
                await kazali.Agent.RequestSelectionOfKazaliMinions(kazaliMinionSelection);

                storyteller.KazaliMinions(kazali, kazaliMinionSelection.Minions);
                foreach (var (player, character) in kazaliMinionSelection.Minions)
                {
                    await grimoire.ChangeCharacter(player, character);
                }
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IReadOnlyCollection<Character> script;
    }
}
