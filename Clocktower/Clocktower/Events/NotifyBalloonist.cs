using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class NotifyBalloonist : IGameEvent
    {
        public NotifyBalloonist(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            foreach (var balloonist in grimoire.PlayersForWhomWeShouldRunAbility(Character.Balloonist))
            {
                await RunBalloonist(balloonist);
            }
        }

        private async Task RunBalloonist(Player balloonist)
        {
            var previousPing = grimoire.Players.FirstOrDefault(player => player.Tokens.HasTokenForPlayer(Token.BalloonistPing, balloonist));
            var ping = await storyteller.GetPlayerForBalloonist(balloonist, previousPing, GetPossibleNewPings(balloonist, previousPing).ToList());

            previousPing?.Tokens.Remove(Token.BalloonistPing, balloonist);
            ping.Tokens.Add(Token.BalloonistPing, balloonist);

            await balloonist.Agent.NotifyBalloonist(ping);
            storyteller.NotifyBalloonist(balloonist, ping);
        }

        private IEnumerable<Player> GetPossibleNewPings(Player balloonist, Player? previousPing)
        {
            if (previousPing == null)
            {
                // All players are possible for first ping.
                return grimoire.Players;
            }

            if (balloonist.DrunkOrPoisoned)
            {
                return grimoire.Players.Where(player => player != previousPing);
            }

            if (CanRegisterAsMultipleCharacterTypes(previousPing))
            {
                // Because of multiple registration, all other players are possible. (Technically you could show the same player again - I don't think this is the intent.)
                return grimoire.Players.Where(player => player != previousPing);
            }

            var previousCharacterType = previousPing.CharacterType;
            return grimoire.Players.Where(player => CanRegisterAsCharacterTypeOtherThan(player, previousCharacterType));
        }

        private static bool CanRegisterAsMultipleCharacterTypes(Player player)
        {
            int count = 0;
            if (player.CanRegisterAsTownsfolk)
            {
                count++;
            }
            if (player.CanRegisterAsOutsider)
            {
                count++;
            }
            if (player.CanRegisterAsMinion)
            {
                count++;
            }
            if (player.CanRegisterAsDemon)
            {
                count++;
            }
            return count > 1;
        }

        private static bool CanRegisterAsCharacterTypeOtherThan(Player player, CharacterType characterType)
        {
            if (characterType != CharacterType.Townsfolk && player.CanRegisterAsTownsfolk)
            {
                return true;
            }
            if (characterType != CharacterType.Outsider && player.CanRegisterAsOutsider)
            {
                return true;
            }
            if (characterType != CharacterType.Minion && player.CanRegisterAsMinion)
            {
                return true;
            }
            if (characterType != CharacterType.Demon && player.CanRegisterAsDemon)
            {
                return true;
            }
            return false;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
