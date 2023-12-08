using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Options;

namespace Clocktower.Events
{
    internal class ChoiceFromImp : IGameEvent
    {
        public ChoiceFromImp(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            foreach (var imp in grimoire.GetLivingPlayers(Character.Imp))
            {
                var options = grimoire.Players.Select(player => new PlayerOption(player)).ToList();
                var choice = (PlayerOption)await imp.Agent.RequestChoiceFromImp(options);
                var target = choice.Player;

                storyteller.ChoiceFromImp(imp, target);
                if (!imp.DrunkOrPoisoned && target.Alive && target.CanBeKilledByDemon)
                {
                    target.Tokens.Add(Token.KilledByDemon);
                    if (target == imp)
                    {   // Star-pass
                        // For now it just goes to the first alive minion.
                        var newImp = grimoire.Players.FirstOrDefault(player => player.Alive && player.CharacterType == CharacterType.Minion);
                        if (newImp != null)
                        {
                            newImp.ChangeCharacter(Character.Imp);
                            storyteller.AssignCharacter(newImp);
                        }
                    }
                }
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
