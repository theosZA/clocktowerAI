using Clocktower.Game;
using Clocktower.Setup;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class ChoiceOfKazaliMinions : IGameEvent
    {
        public ChoiceOfKazaliMinions(IStoryteller storyteller, Grimoire grimoire, IReadOnlyCollection<Character> script)
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

                var kazaliMinions = await kazali.Agent.RequestChoiceOfKazaliMinions(minionCount, possiblePlayers, minionCharacters);

                storyteller.KazaliMinions(kazali, kazaliMinions.MinionAssignment);
                foreach (var (player, character) in kazaliMinions.MinionAssignment)
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
