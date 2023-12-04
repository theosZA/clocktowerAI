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

        public void RunEvent(Action onEventFinished)
        {
            var ravenkeeper = grimoire.GetPlayer(Character.Ravenkeeper);
            if (ravenkeeper == null || !ravenkeeper.Tokens.Contains(Token.DiedAtNight)) // Needs to exclude Godfather and Assassin!
            {
                onEventFinished();
                return;
            }

            var options = grimoire.Players.Select(player => new PlayerOption(player)).ToList();
            ravenkeeper.Agent.RequestChoiceFromRavenkeeper(options, option =>
            {
                var playerOption = option as PlayerOption;
                Debug.Assert(playerOption != null);
                var player = playerOption.Player;

                if (!player.RealCharacter.HasValue)
                {
                    throw new ArgumentException("Player does not have character assigned");
                }
                var character = player.RealCharacter.Value;
                if (character == Character.Recluse)
                {
                    // Hard-coded misinfo
                    character = Character.Imp;
                }
                else if (ravenkeeper.DrunkOrPoisoned)
                {
                    if (!player.Alignment.HasValue)
                    {
                        throw new ArgumentException("Player does not have alignment assigned");
                    }
                    if (player.Alignment.Value == Alignment.Evil)
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
                onEventFinished();
            });
        }

        private IStoryteller storyteller;
        private Grimoire grimoire;
    }
}
