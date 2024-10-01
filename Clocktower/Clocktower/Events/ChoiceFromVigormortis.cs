using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Storyteller;
using Clocktower.Triggers;

namespace Clocktower.Events
{
    internal class ChoiceFromVigormortis : IGameEvent
    {
        public ChoiceFromVigormortis(IStoryteller storyteller, Grimoire grimoire, Deaths deaths)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.deaths = deaths;
        }

        public async Task RunEvent()
        {
            var demons = grimoire.PlayersForWhomWeShouldRunAbility(Character.Vigormortis).ToList();   // Fix the demons first, so that any demons created during this process don't get a kill.
            foreach (var vigormortis in demons)
            {
                await RunVigormortis(vigormortis);
            }
        }

        private async Task RunVigormortis(Player vigormortis)
        {
            var target = await vigormortis.Agent.RequestChoiceFromDemon(Character.Vigormortis, grimoire.Players);
            storyteller.ChoiceFromDemon(vigormortis, target);

            if (!vigormortis.DrunkOrPoisoned)
            {
                if (target.CharacterType == CharacterType.Minion)
                {
                    target.Tokens.Add(Token.MinionKilledByVigormortis, vigormortis);
                }

                await deaths.NightKill(target, vigormortis);

                if (target.CharacterType == CharacterType.Minion || (target.CanRegisterAsMinion && await storyteller.ShouldRegisterAsMinionForVigormortis(vigormortis, target)))
                {
                    await PoisonTownsfolkNeighbour(vigormortis, target);
                }
            }
        }

        private async Task PoisonTownsfolkNeighbour(Player vigormortis, Player minion)
        {
            var players = grimoire.Players.ToList();
            int minionIndex = players.IndexOf(minion);
            var neighbourA = GetClockwiseTownfolkNeighbour(players, minionIndex);
            var neighbourB = GetCounterclockwiseTownfolkNeighbour(players, minionIndex);
            var poisonedNeighbour = await PickPoisonedTownsfolkNeighbour(minion, neighbourA, neighbourB);
            poisonedNeighbour?.Tokens?.Add(Token.PoisonedByVigormortis, vigormortis);
        }

        private static Player? GetClockwiseTownfolkNeighbour(IList<Player> players, int playerIndex)
        {
            for (int step = 1; step < players.Count; step++)
            {
                var clockwisePlayer = players[(playerIndex + step) % players.Count];
                if (clockwisePlayer.CharacterType == CharacterType.Townsfolk)
                {
                    return clockwisePlayer;
                }
            }
            return null;
        }

        private static Player? GetCounterclockwiseTownfolkNeighbour(IList<Player> players, int playerIndex)
        {
            for (int step = 1; step < players.Count; step++)
            {
                var clockwisePlayer = players[(playerIndex + players.Count - step) % players.Count];
                if (clockwisePlayer.CharacterType == CharacterType.Townsfolk)
                {
                    return clockwisePlayer;
                }
            }
            return null;
        }

        private async Task<Player?> PickPoisonedTownsfolkNeighbour(Player minion, Player? neighbourA, Player? neighbourB)
        {
            if (neighbourA == null)
            {
                return neighbourB;
            }
            if (neighbourB == null)
            {
                return neighbourA;
            }
            return await storyteller.GetTownsfolkPoisonedByVigormortis(minion, neighbourA, neighbourB);
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly Deaths deaths;
    }
}
