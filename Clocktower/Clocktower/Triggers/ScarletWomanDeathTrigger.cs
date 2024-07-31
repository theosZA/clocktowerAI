using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Triggers
{
    internal class ScarletWomanDeathTrigger : IDeathTrigger
    {
        public ScarletWomanDeathTrigger(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public Task RunTrigger(DeathInformation deathInformation)
        {
            if (deathInformation.dyingPlayer.CharacterType == CharacterType.Demon && grimoire.Players.Count(player => player.Alive) >= 5)
            {
                var scarletWoman = grimoire.PlayersWithHealthyAbility(Character.Scarlet_Woman).FirstOrDefault(); // shouldn't be more than 1 Scarlet Woman
                if (scarletWoman != null)
                {
                    deathInformation.hasScarletWomanJustBecomeDemon = true;
                    storyteller.ScarletWomanTrigger(deathInformation.dyingPlayer, scarletWoman);
                    grimoire.ChangeCharacter(scarletWoman, deathInformation.dyingPlayer.Character);
                }
            }

            return Task.CompletedTask;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
