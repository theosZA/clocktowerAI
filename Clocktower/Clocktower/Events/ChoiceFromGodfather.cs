using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class ChoiceFromGodfather : IGameEvent
    {
        public ChoiceFromGodfather(IStoryteller storyteller, Grimoire grimoire, Kills kills)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.kills = kills;
        }

        public async Task RunEvent()
        {
            foreach (var godfather in grimoire.PlayersForWhomWeShouldRunAbility(Character.Godfather).WithToken(Token.GodfatherKillsTonight))
            {
                var target = await godfather.Agent.RequestChoiceFromGodfather(grimoire.Players);
                storyteller.ChoiceFromGodfather(godfather, target);

                godfather.Tokens.Remove(Token.GodfatherKillsTonight);
                if (!godfather.DrunkOrPoisoned && target.Alive)
                {
                    await kills.NightKill(target, godfather);
                }
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly Kills kills;
    }
}
