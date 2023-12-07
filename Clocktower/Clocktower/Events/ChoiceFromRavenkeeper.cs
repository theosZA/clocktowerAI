using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Options;
using System.Diagnostics;

namespace Clocktower.Events
{
    internal class ChoiceFromRavenkeeper : IGameEvent
    {
        public ChoiceFromRavenkeeper(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            foreach (var ravenkeeper in grimoire.Players.Where(player => player.Character == Character.Ravenkeeper && player.Tokens.Contains(Token.KilledByDemon)))
            {
                var options = grimoire.Players.Select(player => new PlayerOption(player)).ToList();

                var choice = await ravenkeeper.Agent.RequestChoiceFromRavenkeeper(options);
                var playerOption = choice as PlayerOption;
                Debug.Assert(playerOption != null);

                var player = playerOption.Player;
                var character = player.Character;
                if (character == Character.Recluse)
                {
                    // Hard-coded misinfo
                    character = Character.Imp;
                }
                else if (ravenkeeper.DrunkOrPoisoned)
                {
                    if (player.Alignment == Alignment.Evil)
                    {
                        // Hard-coded bluffs
                        if (character == Character.Imp)
                        {
                            character = Character.Soldier;
                        }
                        else
                        {
                            character = Character.Fortune_Teller;
                        }
                    }
                    else if (player != ravenkeeper)
                    {
                        // Hard-coded misinfo
                        character = Character.Imp;
                    }
                }

                storyteller.ChoiceFromRavenkeeper(ravenkeeper, player, character);
                ravenkeeper.Agent.NotifyRavenkeeper(player, character);

            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
