using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Triggers
{
    internal class ImpStarPassDeathTrigger : IDeathTrigger
    {
        public ImpStarPassDeathTrigger(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunTrigger(DeathInformation deathInformation)
        {
            if (deathInformation.duringDay
                || deathInformation.hasScarletWomanJustBecomeDemon
                || deathInformation.dyingPlayer.Character != Character.Imp
                || deathInformation.dyingPlayer != deathInformation.killingPlayer)
            {
                return;
            }

            var newImp = await GetNewImp();
            if (newImp != null)
            {
                await grimoire.ChangeCharacter(newImp, Character.Imp);
                storyteller.AssignCharacter(newImp);
            }
        }

        private async Task<Player?> GetNewImp()
        {
            var aliveMinions = grimoire.Players.Where(player => player.Alive && player.CharacterType == CharacterType.Minion).ToList();
            return aliveMinions.Count switch
            {
                0 => null,  // Nobody to star-pass to!
                1 => aliveMinions[0],
                _ => await storyteller.GetNewImp(aliveMinions),
            };
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
