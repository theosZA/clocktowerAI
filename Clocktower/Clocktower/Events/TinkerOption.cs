using Clocktower.Agent.Observer;
using Clocktower.Game;
using Clocktower.Storyteller;
using Clocktower.Triggers;

namespace Clocktower.Events
{
    internal class TinkerOption : IGameEvent
    {
        public TinkerOption(IStoryteller storyteller, Grimoire grimoire, Deaths deaths, IGameObserver observers, bool duringDay)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.deaths = deaths;
            this.observers = observers;
            this.duringDay = duringDay;
        }

        /// <summary>
        /// While the Tinker can die at any time, to save from checking in with our storyteller so frequently, we should only check
        /// at the Tinker's spot in the night order, the start of the day (after night deaths have been announced), before nominations,
        /// and after nominations before ending the day.
        /// </summary>
        public async Task RunEvent()
        {
            foreach (var tinker in grimoire.PlayersWithHealthyAbility(Character.Tinker))
            {
                await RunTinker(tinker);
            }
        }

        public async Task RunTinker(Player tinker)
        {
            if (duringDay && grimoire.PlayerToBeExecuted == tinker)
            {   // The Tinker is due to die to execution, so there's no need to prompt now.
                return;
            }

            if (await storyteller.ShouldKillTinker(tinker))
            {
                if (duringDay)
                {
                    await deaths.DayKill(tinker, killer: null);
                    await observers.PlayerDies(tinker);
                }
                else
                {
                    await deaths.NightKill(tinker, killer: null);
                }
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly Deaths deaths;
        private readonly IGameObserver observers;
        private readonly bool duringDay;
    }
}
