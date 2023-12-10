using Clocktower.Game;
using Clocktower.Options;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class ChoiceFromGodfather : IGameEvent
    {
        public ChoiceFromGodfather(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            foreach (var godfather in grimoire.GetLivingPlayers(Character.Godfather).Where(player => player.Tokens.Contains(Token.GodfatherKillsTonight)))
            {
                var choice = (PlayerOption)await godfather.Agent.RequestChoiceFromGodfather(grimoire.Players.ToOptions());
                var target = choice.Player;
                storyteller.ChoiceFromGodfather(godfather, target);

                godfather.Tokens.Remove(Token.GodfatherKillsTonight);
                if (!godfather.DrunkOrPoisoned && target.Alive)
                {
                    new Kills(storyteller, grimoire).NightKill(target);
                }
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
