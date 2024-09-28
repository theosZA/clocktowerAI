using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class ChoiceFromOgre : IGameEvent
    {
        public ChoiceFromOgre(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            foreach (var ogre in grimoire.PlayersForWhomWeShouldRunAbility(Character.Ogre))
            {
                await RunOgre(ogre);
            }   
        }

        public async Task RunOgre(Player ogre)
        {
            var target = await ogre.Agent.RequestChoiceFromOgre(grimoire.Players);
            storyteller.ChoiceFromOgre(ogre, target);
            var newAlignment = await NewAlignment(ogre, target);
            await ogre.ChangeAlignment(newAlignment, notifyAgent: false);
        }

        private async Task<Alignment> NewAlignment(Player ogre, Player target)
        {
            if (!target.CanRegisterAsEvil)
            {
                return Alignment.Good;
            }
            if (!target.CanRegisterAsGood)
            {
                return Alignment.Evil;

            }
            return (await storyteller.ShouldRegisterAsEvilForOgre(ogre, target)) ? Alignment.Evil : Alignment.Good;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
