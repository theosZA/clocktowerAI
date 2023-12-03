using Clocktower.Agent;
using Clocktower.Game;

namespace Clocktower.Night
{
    internal class ChoiceFromRavenkeeper : INightEvent
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

            ravenkeeper.Agent.RequestChoiceFromRavenkeeper(grimoire.Players, player =>
            {
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
