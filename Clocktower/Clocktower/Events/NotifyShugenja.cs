using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Options;

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

            var evilStepsClockwise = allPlayersClockwise.Select((player, i) => (player, i + 1))
                                                        .First(pair => pair.player.Alignment == Alignment.Evil).Item2;
            var evilStepsCounterclockwise = allPlayersCounterclockwise.Select((player, i) => (player, i + 1))
                                                                      .First(pair => pair.player.Alignment == Alignment.Evil).Item2;

            if (allPlayersClockwise.Any(player => player.Character == Character.Recluse))
            {
                var recluseStepsClockwise = allPlayersClockwise.Select((player, i) => (player, i + 1))
                                                               .First(pair => pair.player.Character == Character.Recluse).Item2;
                var recluseStepsCounterclockwise = allPlayersCounterclockwise.Select((player, i) => (player, i + 1))
                                                                             .First(pair => pair.player.Character == Character.Recluse).Item2;

                if (recluseStepsClockwise <= evilStepsCounterclockwise && evilStepsCounterclockwise < evilStepsClockwise)
                {   // If we count the Recluse as evil, then this would be clockwise (or equal) instead of counter-clockwise.
                    return await GetDirectionFromStoryteller(shugenja);
                }
                if (recluseStepsCounterclockwise <= evilStepsClockwise && evilStepsClockwise < evilStepsCounterclockwise)
                {   // If we count the Recluse as evil, then this would be counter-clockwise (or equal) instead of clockwise.
                    return await GetDirectionFromStoryteller(shugenja);
                }
                // If we counted the Recluse as evil, it wouldn't change anything. Proceed to default handling.
            }

            if (evilStepsClockwise == evilStepsCounterclockwise)
            {
                return await GetDirectionFromStoryteller(shugenja);
            }
            return evilStepsClockwise < evilStepsCounterclockwise;
        }

        private async Task<bool> GetDirectionFromStoryteller(Player shugenja)
        {
            return await storyteller.GetShugenjaDirection(shugenja, grimoire, new IOption[] { new ClockwiseOption(), new CounterclockwiseOption() }) is ClockwiseOption;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
