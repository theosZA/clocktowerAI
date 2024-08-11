using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Triggers
{
    internal class SweetheartDeathTrigger : IDeathTrigger
    {
        public SweetheartDeathTrigger(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunTrigger(DeathInformation deathInformation)
        {
            if (deathInformation.dyingPlayer.HasHealthyAbility(Character.Sweetheart))
            {
                var sweetheartDrunk = await storyteller.GetSweetheartDrunk(grimoire.Players);
                sweetheartDrunk.Tokens.Add(Token.SweetheartDrunk, deathInformation.dyingPlayer);
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
