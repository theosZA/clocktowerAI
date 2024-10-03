using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class NotifyHighPriestess : IGameEvent
    {
        public NotifyHighPriestess(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            foreach (var highPriestess in grimoire.PlayersForWhomWeShouldRunAbility(Character.High_Priestess))
            {
                await RunHighPriestess(highPriestess);
            }
        }

        private async Task RunHighPriestess(Player highPriestess)
        {
            var ping = await storyteller.GetPlayerForHighPriestess(highPriestess, grimoire.Players);
            await highPriestess.Agent.NotifyHighPriestess(ping);
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
