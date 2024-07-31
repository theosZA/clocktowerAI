using Clocktower.Game;
using Clocktower.Storyteller;
using Clocktower.Agent;
using Clocktower.Triggers;

namespace Clocktower.Events
{
    internal class ChoiceFromOjo : IGameEvent
    {
        public ChoiceFromOjo(IStoryteller storyteller, Grimoire grimoire, Deaths deaths, IReadOnlyCollection<Character> scriptCharacters)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.deaths = deaths;
            this.scriptCharacters = scriptCharacters;
        }

        public async Task RunEvent()
        {
            foreach (var ojo in grimoire.PlayersForWhomWeShouldRunAbility(Character.Ojo))
            {
                await RunOjo(ojo);
            }
        }

        private async Task RunOjo(Player ojo)
        {
            var (targetCharacter, victims) = await GetOjoVictims(ojo);
            var ojoVictims = victims.ToList();
            storyteller.ChoiceFromOjo(ojo, targetCharacter, ojoVictims);
            foreach (var victim in ojoVictims)
            {
                await deaths.NightKill(victim, ojo);
            }
        }

        private async Task<(Character targetCharacter, IEnumerable<Player> victims)> GetOjoVictims(Player ojo)
        {
            var targetCharacter = await ojo.Agent.RequestChoiceFromOjo(scriptCharacters);

            if (ojo.DrunkOrPoisoned)
            {
                return (targetCharacter, Array.Empty<Player>());
            }

            var matchingPlayers = grimoire.Players.Where(player => player.CanRegisterAs(targetCharacter)).ToList();
            return matchingPlayers.Count switch
            {
                0 => (targetCharacter, await storyteller.GetOjoVictims(ojo, targetCharacter, grimoire.Players.Where(player => player.Alive))),
                1 => (targetCharacter, new[] { matchingPlayers[0] }),
                _ => (targetCharacter, await GetSingleOjoVictim(ojo, targetCharacter, matchingPlayers))
            };
        }

        private async Task<IEnumerable<Player>> GetSingleOjoVictim(Player ojo, Character targetCharacter, IEnumerable<Player> matchingPlayers)
        {
            var matchingLivingPlayers = matchingPlayers.Where(player => player.Alive).ToList();
            return matchingLivingPlayers.Count switch
            {
                0 => Array.Empty<Player>(),
                1 => matchingLivingPlayers,
                _ => new Player[] { await storyteller.GetOjoVictim(ojo, targetCharacter, matchingLivingPlayers) }
            };
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly Deaths deaths;
        private readonly IReadOnlyCollection<Character> scriptCharacters;
    }
}
