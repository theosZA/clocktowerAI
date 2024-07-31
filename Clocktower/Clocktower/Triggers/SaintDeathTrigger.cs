using Clocktower.Game;

namespace Clocktower.Triggers
{
    internal class SaintDeathTrigger : IDeathTrigger
    {
        public SaintDeathTrigger(Grimoire grimoire)
        {
            this.grimoire = grimoire;
        }

        public Task RunTrigger(DeathInformation deathInformation)
        {
            if (deathInformation.executed && deathInformation.dyingPlayer.HasHealthyAbility(Character.Saint))
            {
                grimoire.EndGame(deathInformation.dyingPlayer.Alignment == Alignment.Good ? Alignment.Evil : Alignment.Good);
            }
            return Task.CompletedTask;
        }

        private readonly Grimoire grimoire;
    }
}
