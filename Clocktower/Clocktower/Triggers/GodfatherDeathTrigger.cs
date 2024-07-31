using Clocktower.Game;

namespace Clocktower.Triggers
{
    internal class GodfatherDeathTrigger : IDeathTrigger
    {
        public GodfatherDeathTrigger(Grimoire grimoire)
        {
            this.grimoire = grimoire;
        }

        public Task RunTrigger(DeathInformation deathInformation)
        {
            if (deathInformation.duringDay && deathInformation.dyingPlayer.CharacterType == CharacterType.Outsider)
            {
                foreach (var godfather in grimoire.PlayersForWhomWeShouldRunAbility(Character.Godfather))
                {
                    godfather.Tokens.Add(Token.GodfatherKillsTonight, deathInformation.dyingPlayer);
                }
            }
            return Task.CompletedTask;
        }

        private readonly Grimoire grimoire;
    }
}
