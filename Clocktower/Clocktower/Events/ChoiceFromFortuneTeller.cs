using Clocktower.Game;
using Clocktower.Options;
using Clocktower.Storyteller;

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
                var (targetA, targetB) = (await fortuneTeller.Agent.RequestChoiceFromFortuneTeller(grimoire.Players.ToTwoPlayersOptions())).GetTwoPlayers();
                bool reading = await GetReading(fortuneTeller, targetA, targetB);
                fortuneTeller.Agent.NotifyFortuneTeller(targetA, targetB, reading);
            }
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
            if (targetA.CanRegisterAsDemon || targetB.CanRegisterAsDemon)
            {
                return await GetReadingFromStoryteller(fortuneTeller, targetA, targetB);
            }
            return false;
        }

        private async Task<bool> GetReadingFromStoryteller(Player fortuneTeller, Player targetA, Player targetB)
        {
            return await storyteller.GetFortuneTellerReading(fortuneTeller, targetA, targetB, OptionsBuilder.YesOrNo) is YesOption;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
