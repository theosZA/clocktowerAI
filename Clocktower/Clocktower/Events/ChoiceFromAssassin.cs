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
                var options = grimoire.Players.Select(player => (IOption)new PlayerOption(player))
                                              .Prepend(new PassOption())
                                              .ToList();

                var choice = await assassin.Agent.RequestChoiceFromAssassin(options);
                var target = choice is PlayerOption playerOption ? playerOption.Player : null;
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
