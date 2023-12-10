using Clocktower.Agent;
using Clocktower.Game;
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
            foreach (var fortuneTeller in grimoire.GetLivingPlayers(Character.Fortune_Teller))
            {
                var (targetA, targetB) = await fortuneTeller.Agent.RequestChoiceFromFortuneTeller(grimoire.Players);
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
            if (targetA.CharacterType == CharacterType.Demon || targetB.CharacterType == CharacterType.Demon)
            {
                return true;                
            }
            if (IsRedHerring(fortuneTeller, targetA) || IsRedHerring(fortuneTeller, targetB))
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
            return await storyteller.GetFortuneTellerReading(fortuneTeller, targetA, targetB);
        }

        private static bool IsRedHerring(Player fortuneTeller, Player target)
        {
            if (fortuneTeller.Tokens.Contains(Token.IsThePhilosopher))
            {
                return target.Tokens.Contains(Token.PhilosopherFortuneTellerRedHerring);
            }
            else
            {
                return target.Tokens.Contains(Token.FortuneTellerRedHerring);
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
