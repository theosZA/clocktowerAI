using Clocktower.Agent;
using Clocktower.Game;

namespace Clocktower.Night
{
    internal class NotifySteward : INightEvent
    {
        public NotifySteward(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public void RunEvent(Action onEventFinished)
        {
            var steward = grimoire.GetAlivePlayer(Character.Steward);
            if (steward != null)
            {
                // For now we give them a hardcoded player.
                var stewardTarget = steward.DrunkOrPoisoned ? grimoire.GetRequiredPlayer(Character.Imp) : grimoire.GetRequiredPlayer(Character.Ravenkeeper);
                steward.Agent.NotifySteward(stewardTarget);
                storyteller.NotifySteward(steward, stewardTarget);
            }

            onEventFinished();
        }

        private IStoryteller storyteller;
        private Grimoire grimoire;
    }
}
