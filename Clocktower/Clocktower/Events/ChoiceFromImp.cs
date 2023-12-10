using Clocktower.Game;
using Clocktower.Options;
using Clocktower.Storyteller;

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
            var imps = grimoire.GetLivingPlayers(Character.Imp).ToList();   // Fix the imp(s) first, so that minions who receive a star-pass don't get to kill.
            foreach (var imp in imps)
            {
                var options = grimoire.Players.ToOptions();
                var choice = (PlayerOption)await imp.Agent.RequestChoiceFromImp(options);
                var target = choice.Player;

                storyteller.ChoiceFromImp(imp, target);
                if (!imp.DrunkOrPoisoned && target.Alive && target.CanBeKilledByDemon)
                {
                    if (target == imp)
                    {
                        await StarPass(target);
                    }
                    else
                    {
                        new Kills(storyteller, grimoire).DemonKill(target);
                    }
                }
            }
        }

        private async Task StarPass(Player dyingImp)
        {
            // We need to check if a Scarlet Woman will become the new Imp. If so, that will already be handled.
            bool scarletWomanCatchesStarPass = grimoire.Players.Count(player => player.Alive) >= 5 && grimoire.Players.Any(player => player.Character == Character.Scarlet_Woman && !player.DrunkOrPoisoned);

            new Kills(storyteller, grimoire).DemonKill(dyingImp);
            if (scarletWomanCatchesStarPass)
            {
                return;
            }

            var newImp = await GetNewImp();
            if (newImp == null)
            {
                return;
            }
            grimoire.ChangeCharacter(newImp, Character.Imp);
            storyteller.AssignCharacter(newImp);
        }

        private async Task<Player?> GetNewImp()
        {
            var aliveMinions = grimoire.Players.Where(player => player.Alive && player.CharacterType == CharacterType.Minion).ToList();
            switch (aliveMinions.Count)
            {
                case 0: // Nobody to star-pass to!
                    return null;

                case 1:
                    return aliveMinions[0];

                default:
                    var choice = (PlayerOption)await storyteller.GetNewImp(aliveMinions.ToOptions());
                    return choice.Player;
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
