using Clocktower.Agent;
using Clocktower.Game;

namespace Clocktower.Night
{
    internal class MinionInformation : INightEvent
    {
        public MinionInformation(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public void RunEvent(Action onEventFinished)
        {
            var demon = grimoire.GetDemon();
            var minions = grimoire.GetMinions().ToList();
            foreach (var minion in minions)
            {
                minion.Agent.MinionInformation(demon, minions.Except(new[] { minion }).ToList());
                storyteller.MinionInformation(minion, demon, minions.Except(new[] { minion }).ToList());
            }

            onEventFinished();
        }

        private IStoryteller storyteller;
        private Grimoire grimoire;
    }
}
