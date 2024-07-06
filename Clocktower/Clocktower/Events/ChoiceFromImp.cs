using Clocktower.Game;
using Clocktower.Storyteller;
using Clocktower.Agent;

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
                var target = await imp.Agent.RequestChoiceFromImp(grimoire.Players);

                storyteller.ChoiceFromImp(imp, target);
                if (!imp.DrunkOrPoisoned && target.Alive)
                {
                    await new Kills(storyteller, grimoire).NightKill(target, imp);
                }
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
