using Clocktower.Game;
using Clocktower.Storyteller;
using Clocktower.Agent;
using Clocktower.Triggers;

namespace Clocktower.Events
{
    internal class ChoiceFromPukka : IGameEvent
    {
        public ChoiceFromPukka(IStoryteller storyteller, Grimoire grimoire, Deaths deaths)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.deaths = deaths;
        }

        public async Task RunEvent()
        {
            var demons = grimoire.PlayersForWhomWeShouldRunAbility(Character.Pukka).ToList();   // Fix the demons first, so that any demons created during this process don't get a kill.
            foreach (var pukka in demons)
            {
                await RunPukka(pukka);
            }
        }

        private async Task RunPukka(Player pukka)
        {
            var target = await pukka.Agent.RequestChoiceFromPukka(grimoire.Players);
            storyteller.ChoiceFromPukka(pukka, target);

            if (!pukka.DrunkOrPoisoned)
            {
                var previousTarget = grimoire.Players.FirstOrDefault(player => player.Tokens.HasTokenForPlayer(Token.PoisonedByPukka, pukka));
                PoisonPukkaTarget(pukka, target);
                if (previousTarget != null)
                {
                    await deaths.NightKill(previousTarget, pukka);
                    target.Tokens.Remove(Token.PoisonedByPukka, previousTarget);
                }
            }
        }

        private void PoisonPukkaTarget(Player pukka, Player target)
        {
            if (target.Tokens.HasTokenForPlayer(Token.PoisonedByPukka, pukka))
            {   // You can't add a second Pukka poison token on the target.
                return;
            }

            if (target.ProtectedFromDemonKill)
            {
                return;
            }

            target.Tokens.Add(Token.PoisonedByPukka, pukka);
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly Deaths deaths;
    }
}
