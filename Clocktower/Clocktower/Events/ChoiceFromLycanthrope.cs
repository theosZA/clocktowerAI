using Clocktower.Game;
using Clocktower.Storyteller;
using Clocktower.Agent;
using Clocktower.Triggers;

namespace Clocktower.Events
{
    internal class ChoiceFromLycanthrope : IGameEvent
    {
        public ChoiceFromLycanthrope(IStoryteller storyteller, Grimoire grimoire, Deaths deaths)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.deaths = deaths;
        }

        public async Task RunEvent()
        {
            foreach (var lycanthrope in grimoire.PlayersForWhomWeShouldRunAbility(Character.Lycanthrope))
            {
                await RunLycanthrope(lycanthrope);
            }
        }

        private async Task RunLycanthrope(Player lycanthrope)
        {
            var target = await lycanthrope.Agent.RequestChoiceFromLycanthrope(grimoire.Players.Where(player => player.Alive));

            if (!await IsSuccessfulLycanthropeKill(lycanthrope, target))
            {   // The kill failed due to the target being evil or not registering as good.
                storyteller.ChoiceFromLycanthrope(lycanthrope, target, success: false);
                return;
            }

            var deadPlayer = await deaths.NightKill(target, lycanthrope, async redirectedTarget => await IsSuccessfulLycanthropeKill(lycanthrope, redirectedTarget));
            if (deadPlayer == null)
            {   // The kill failed due to character abilities or was redirected to a new target which was evil or not registering as good.
                storyteller.ChoiceFromLycanthrope(lycanthrope, target, success: false);
                return;
            }

            storyteller.ChoiceFromLycanthrope(lycanthrope, deadPlayer, success: true);
            if (deadPlayer != lycanthrope)
            {
                deadPlayer.Tokens.Add(Token.KilledByLycanthrope, lycanthrope);
            }
        }

        private async Task<bool> IsSuccessfulLycanthropeKill(Player lycanthrope, Player target)
        {
            if (lycanthrope.DrunkOrPoisoned)
            {
                return false;
            }

            if (!target.CanRegisterAsGood)
            {
                return false;
            }

            if (!target.CanRegisterAsEvil)
            {
                return true;
            }

            return await storyteller.ShouldRegisterAsGoodForLycanthrope(lycanthrope, target);
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly Deaths deaths;
    }
}
