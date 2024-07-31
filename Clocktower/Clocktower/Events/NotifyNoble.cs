using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class NotifyNoble : IGameEvent
    {
        public NotifyNoble(IStoryteller storyteller, Grimoire grimoire, Random random)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.random = random;
        }

        public async Task RunEvent()
        {
            foreach (var noble in grimoire.PlayersForWhomWeShouldRunAbility(Character.Noble))
            {
                await RunEvent(noble);
            }
        }

        public async Task RunEvent(Player noble)
        {
            var nobleInfo = (await storyteller.GetNobleInformation(noble, GetEvilPlayers(noble), GetGoodPlayers(noble))).ToList();
            foreach (var noblePing in nobleInfo)
            {
                noblePing.Tokens.Add(Token.NoblePing, noble);
            }
            nobleInfo.Shuffle(random);
            await noble.Agent.NotifyNoble(nobleInfo);
            storyteller.NotifyNoble(noble, nobleInfo);
        }

        public IEnumerable<Player> GetEvilPlayers(Player noble)
        {
            if (noble.DrunkOrPoisoned)
            {
                return grimoire.Players.Where(player => player != noble);
            }

            return grimoire.Players.Where(player => player != noble && player.CanRegisterAsEvil);
        }

        public IEnumerable<Player> GetGoodPlayers(Player noble)
        {
            if (noble.DrunkOrPoisoned)
            {
                return grimoire.Players.Where(player => player != noble);
            }

            return grimoire.Players.Where(player => player != noble && player.CanRegisterAsGood);
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly Random random;
    }
}
