using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Options;

namespace Clocktower.Events
{
    internal class ChoiceFromFortuneTeller : IGameEvent
    {
        public ChoiceFromFortuneTeller(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            foreach (var fortuneTeller in grimoire.Players.Where(player => player.Character == Character.Fortune_Teller))
            {
                var options = GetOptions().ToList();
                var targets = (TwoPlayersOption)await fortuneTeller.Agent.RequestChoiceFromFortuneTeller(options);
                bool reading = await GetReading(fortuneTeller, targets.PlayerA, targets.PlayerB);
                fortuneTeller.Agent.NotifyFortuneTeller(targets.PlayerA, targets.PlayerB, reading);
            }
        }

        private IEnumerable<IOption> GetOptions()
        {
            return grimoire.Players.SelectMany(playerA => grimoire.Players.Select(playerB => (playerA, playerB)))
                                   .Where(pair => pair.playerA != pair.playerB)
                                   .Select(pair => new TwoPlayersOption(pair.playerA, pair.playerB));
        }

        private async Task<bool> GetReading(Player fortuneTeller, Player targetA, Player targetB)
        {
            if (fortuneTeller.DrunkOrPoisoned)
            {
                return await GetReadingFromStoryteller(fortuneTeller, targetA, targetB);
            }
            if (targetA.CharacterType == CharacterType.Demon || targetB.CharacterType == CharacterType.Demon || targetA.Tokens.Contains(Token.FortuneTellerRedHerring) || targetB.Tokens.Contains(Token.FortuneTellerRedHerring))
            {
                return true;                
            }
            if (targetA.Character == Character.Recluse || targetB.Character == Character.Recluse)
            {
                return await GetReadingFromStoryteller(fortuneTeller, targetA, targetB);
            }
            return false;
        }

        private async Task<bool> GetReadingFromStoryteller(Player fortuneTeller, Player targetA, Player targetB)
        {
            var option = await storyteller.GetFortuneTellerReading(fortuneTeller, targetA, targetB, new IOption[] { new NoOption(), new YesOption() });
            return option is YesOption;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
