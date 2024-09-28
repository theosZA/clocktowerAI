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
            var target = await ogre.Agent.RequestChoiceFromOgre(grimoire.Players.Where(player => player != ogre));
            storyteller.ChoiceFromOgre(ogre, target);
            if (!ShouldProcessOgre(ogre))
            {
                return;
            }

            var newAlignment = await NewAlignment(ogre, target);
            await ogre.ChangeAlignment(newAlignment, notifyAgent: (target.Character == Character.Recluse && newAlignment == Alignment.Evil));
        }

        private bool ShouldProcessOgre(Player ogre)
        {
            // The Ogre should be processed even if drunk or poisoned.
            // The exception is when it's another character that believes they gained the Ogre ability, but never did.

            if (ogre.Tokens.HasToken(Token.IsTheBadPhilosopher))
            {
                return false;
            }

            if (ogre.Tokens.HasToken(Token.CannibalPoisoned))
            {
                return false;
            }

            if (ogre.RealCharacter == Character.Drunk || ogre.RealCharacter == Character.Marionette)
            {
                return false;
            }

            return true;
        }

        private async Task<Alignment> NewAlignment(Player ogre, Player target)
        {
            if (!target.CanRegisterAsEvil)
            {
                return Alignment.Good;
            }
            if (target.Alignment == Alignment.Evil)
            {
                return Alignment.Evil;

            }
            return (await storyteller.ShouldRegisterAsEvilForOgre(ogre, target)) ? Alignment.Evil : Alignment.Good;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
