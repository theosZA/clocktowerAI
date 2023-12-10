using Clocktower.Game;
using Clocktower.Observer;
using Clocktower.Options;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class TinkerOption : IGameEvent
    {
        public TinkerOption(IStoryteller storyteller, Grimoire grimoire, ObserverCollection observers, bool duringDay)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
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
            foreach (var tinker in grimoire.GetLivingPlayers(Character.Tinker).Where(player => !player.DrunkOrPoisoned))
            {
                if (await storyteller.ShouldKillTinker(tinker, OptionsBuilder.YesOrNo) is YesOption)
                {
                    var kills = new Kills(storyteller, grimoire);
                    if (duringDay)
                    {
                        kills.DayKill(tinker);
                        observers.PlayerDies(tinker);
                    }
                    else
                    {
                        kills.NightKill(tinker);
                    }
                }
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly ObserverCollection observers;
        private readonly bool duringDay;
    }
}
