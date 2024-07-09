using Clocktower.Game;
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
                await RunEvent(shugenja);
            }
        }

        public async Task RunEvent(Player shugenja)
        {
            var direction = await GetDirection(shugenja);
            await shugenja.Agent.NotifyShugenja(direction);
            storyteller.NotifyShugenja(shugenja, direction);
        }

        private async Task<Direction> GetDirection(Player shugenja)
        {
            var possibleDirections = GetPossibleShugenjaDirections(shugenja).Distinct().ToList();
            if (possibleDirections.Count == 1)
            {
                return possibleDirections[0];
            }
            return await storyteller.GetShugenjaDirection(shugenja, grimoire);
        }

        private IEnumerable<Direction> GetPossibleShugenjaDirections(Player shugenja)
        {
            if (shugenja.DrunkOrPoisoned)
            {
                yield return Direction.Clockwise;
                yield return Direction.Counterclockwise;
            }

            // Consider each step count until we get to the far side of the Grimoire.
            // Each time we have a character that can misregister we include them as a possible direction.
            // We stop when we have a evil player (in either direction) who can't misregister.
            var players = grimoire.Players.ToList();
            int shugenjaPosition = players.IndexOf(shugenja);
            bool nonMisregisteringEvil = false;
            for (int step = 1; step < grimoire.Players.Count / 2 && !nonMisregisteringEvil; step++)
            {
                var clockwisePlayer = players[(shugenjaPosition + step) % grimoire.Players.Count];
                if (clockwisePlayer.CanRegisterAsEvil)
                {
                    yield return Direction.Clockwise;
                    if (!clockwisePlayer.CanRegisterAsGood)
                    {
                        nonMisregisteringEvil = true;
                    }
                }
                var counterclockwisePlayer = players[(shugenjaPosition - step + grimoire.Players.Count) % grimoire.Players.Count];
                if (counterclockwisePlayer.CanRegisterAsEvil)
                {
                    yield return Direction.Counterclockwise;
                    if (!counterclockwisePlayer.CanRegisterAsGood)
                    {
                        nonMisregisteringEvil = true;
                    }
                }
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
