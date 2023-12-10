using Clocktower.Game;
using Clocktower.Options;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class NotifyShugenja : IGameEvent
    {
        public NotifyShugenja(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            foreach (var shugenja in grimoire.GetLivingPlayers(Character.Shugenja))
            {
                var clockwise = await GetDirection(shugenja);

                shugenja.Agent.NotifyShugenja(clockwise);
                storyteller.NotifyShugenja(shugenja, clockwise);
            }
        }

        private async Task<bool> GetDirection(Player shugenja)
        {
            if (shugenja.DrunkOrPoisoned)
            {
                return await GetDirectionFromStoryteller(shugenja);
            }

            IReadOnlyCollection<Player> allPlayersClockwise = grimoire.GetAllPlayersEndingWithPlayer(shugenja).SkipLast(1).ToList();
            IReadOnlyCollection<Player> allPlayersCounterclockwise = allPlayersClockwise.Reverse().ToList();

            var minEvilStepsClockwise = allPlayersClockwise.Select((player, i) => (player, i + 1))
                                                           .First(pair => pair.player.CanRegisterAsEvil).Item2;
            var maxEvilStepsClockwise = allPlayersClockwise.Select((player, i) => (player, i + 1))
                                                           .First(pair => pair.player.Alignment == Alignment.Evil).Item2;
            var minEvilStepsCounterclockwise = allPlayersCounterclockwise.Select((player, i) => (player, i + 1))
                                                                         .First(pair => pair.player.CanRegisterAsEvil).Item2;
            var maxEvilStepsCounterclockwise = allPlayersCounterclockwise.Select((player, i) => (player, i + 1))
                                                                         .First(pair => pair.player.Alignment == Alignment.Evil).Item2;

            if (minEvilStepsClockwise <= maxEvilStepsCounterclockwise && maxEvilStepsCounterclockwise < maxEvilStepsClockwise)
            {   // If we count the Recluse as evil, then this would be clockwise (or equal) instead of counter-clockwise.
                return await GetDirectionFromStoryteller(shugenja);
            }
            if (minEvilStepsCounterclockwise <= maxEvilStepsClockwise && maxEvilStepsClockwise < maxEvilStepsCounterclockwise)
            {   // If we count the Recluse as evil, then this would be counter-clockwise (or equal) instead of clockwise.
                return await GetDirectionFromStoryteller(shugenja);
            }
            // Even if we count the Recluse as evil, it woun't change anything. Proceed to default handling.

            if (maxEvilStepsClockwise == maxEvilStepsCounterclockwise)
            {
                return await GetDirectionFromStoryteller(shugenja);
            }
            return maxEvilStepsClockwise < maxEvilStepsCounterclockwise;
        }

        private async Task<bool> GetDirectionFromStoryteller(Player shugenja)
        {
            return await storyteller.GetShugenjaDirection(shugenja, grimoire, new IOption[] { new ClockwiseOption(), new CounterclockwiseOption() }) is ClockwiseOption;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
