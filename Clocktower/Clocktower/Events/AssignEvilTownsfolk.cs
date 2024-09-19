using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class AssignEvilTownsfolk : IGameEvent
    {
        public AssignEvilTownsfolk(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            foreach (var bountyHunter in grimoire.PlayersForWhomWeShouldRunAbility(Character.Bounty_Hunter))
            {
                var evilTownsfolk = await GetEvilTownsfolkFor(bountyHunter);
                if (evilTownsfolk != null)
                {
                    await evilTownsfolk.ChangeAlignment(Alignment.Evil);
                }
            }
        }

        public async Task<Player?> GetEvilTownsfolkFor(Player bountyHunter)
        {
            if (bountyHunter.RealCharacter == Character.Bounty_Hunter)
            {
                return await storyteller.GetEvilTownsfolk(bountyHunter, grimoire.Players.WithCharacterType(CharacterType.Townsfolk));
            }

            if (bountyHunter.Character == Character.Bounty_Hunter && bountyHunter.Tokens.HasToken(Token.IsThePhilosopher))
            {   // A Philosopher-Bounty Hunter may optionally turn a Townsfolk evil.
                // Normally this is done when running the Philosopher ability later in the night order, but if the Demon is a Kazali, a Townsfolk is only
                // turned evil after Minions are chosen by the Kazali.
                return await storyteller.GetEvilTownsfolk(bountyHunter, grimoire.Players.WithCharacterType(CharacterType.Townsfolk), optional: true);
            }

            // Other Bounty Hunters (like a Drunk-Bounty Hunter or Marionette-Bounty Hunter) can't actually turn a Townsfolk evil.
            return null;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
