using Clocktower.Game;
using Clocktower.Storyteller;
using Clocktower.Triggers;

namespace Clocktower.Events
{
    internal class CheckAcrobat : IGameEvent
    {
        public CheckAcrobat(IStoryteller storyteller, Grimoire grimoire, Deaths deaths)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.deaths = deaths;
        }

        public async Task RunEvent()
        {
            foreach (var acrobat in grimoire.PlayersWithHealthyAbility(Character.Acrobat))
            {
                await RunEvent(acrobat);
            }
        }

        public async Task RunEvent(Player acrobat)
        {
            var players = grimoire.Players.ToList();
            var acrobatIndex = players.IndexOf(acrobat);
            var clockwiseNeighbour = GetLivingGoodClockwiseNeighbour(players, acrobatIndex);
            if (clockwiseNeighbour?.DrunkOrPoisoned == true)
            {
                await deaths.NightKill(acrobat, killer: null);
                storyteller.AcrobatTrigger(acrobat, clockwiseNeighbour);
                return;
            }
            var anticlockwiseNeighbour = GetLivingGoodAnticlockwiseNeighbour(players, acrobatIndex);
            if (anticlockwiseNeighbour?.DrunkOrPoisoned == true)
            {
                await deaths.NightKill(acrobat, killer: null);
                storyteller.AcrobatTrigger(acrobat, anticlockwiseNeighbour);
            }
        }

        public static Player? GetLivingGoodClockwiseNeighbour(List<Player> players, int index)
        {
            int startIndex = index + 1;
            for (int i = startIndex; i != index; i = (i + 1) % players.Count)
            {
                if (players[i].Alive && players[i].Alignment == Alignment.Good)
                {
                    return players[i];
                }
            }
            return null;
        }

        public static Player? GetLivingGoodAnticlockwiseNeighbour(List<Player> players, int index)
        {
            int startIndex = (index + players.Count - 1) % players.Count;
            for (int i = startIndex; i != index; i = (i + players.Count - 1) % players.Count)
            {
                if (players[i].Alive && players[i].Alignment == Alignment.Good)
                {
                    return players[i];
                }
            }
            return null;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly Deaths deaths;
    }
}
