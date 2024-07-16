using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class ShowGrimoireToSpy : IGameEvent
    {
        public ShowGrimoireToSpy(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            // What do we do with a drunk or poisoned Spy?
            // In theory they see a bad Grimoire, but that doesn't seem reasonable to implement.
            // So they just won't get to see the Grimoire.

            foreach (var spy in grimoire.GetHealthyPlayersWithRealAbility(Character.Spy))
            {
                await RunEvent(spy);
            }
        }

        public async Task RunEvent(Player spy)
        {
            await spy.Agent.ShowGrimoireToSpy(grimoire);
            storyteller.ShowGrimoireToSpy(spy, grimoire);
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
