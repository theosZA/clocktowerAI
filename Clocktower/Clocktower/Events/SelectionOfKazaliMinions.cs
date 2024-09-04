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
                var characterLimitations = new Dictionary<Character, IReadOnlyCollection<Player>>();
                if (script.Contains(Character.Marionette))
                {
                    var players = grimoire.Players.ToList();
                    int kazaliIndex = players.IndexOf(kazali);
                    var neighbours = new[]
                    {
                        players[(kazaliIndex + 1) % players.Count],
                        players[(kazaliIndex + players.Count - 1) % players.Count]
                    };
                    characterLimitations.Add(Character.Marionette, neighbours);
                }

                var kazaliMinionSelection = new KazaliMinionsSelection(minionCount, possiblePlayers, minionCharacters, characterLimitations);
                await kazali.Agent.RequestSelectionOfKazaliMinions(kazaliMinionSelection);

                storyteller.KazaliMinions(kazali, kazaliMinionSelection.Minions);
                foreach (var (player, character) in kazaliMinionSelection.Minions)
                {
                    await grimoire.ChangeCharacter(player, character);
                    storyteller.AssignCharacter(player);
                }
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IReadOnlyCollection<Character> script;
    }
}
