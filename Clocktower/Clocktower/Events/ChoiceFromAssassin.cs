using Clocktower.Game;
using Clocktower.Options;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class ChoiceFromAssassin : IGameEvent
    {
        public ChoiceFromAssassin(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            foreach (var assassin in grimoire.GetLivingPlayers(Character.Assassin).Where(player => !player.Tokens.Contains(Token.UsedOncePerGameAbility)))
            {
                var options = grimoire.Players.ToOptions()
                                              .Prepend(new PassOption())
                                              .ToList();

                var target = (await assassin.Agent.RequestChoiceFromAssassin(options)).GetPlayerOptional();
                storyteller.ChoiceFromAssassin(assassin, target);

                if (target != null)
                {
                    assassin.Tokens.Add(Token.UsedOncePerGameAbility);
                    if (!assassin.DrunkOrPoisoned && target.Alive)
                    {
                        new Kills(storyteller, grimoire).NightKill(target);
                    }
                }
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
