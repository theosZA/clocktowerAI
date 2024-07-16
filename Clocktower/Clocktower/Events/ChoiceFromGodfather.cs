using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class ChoiceFromGodfather : IGameEvent
    {
        public ChoiceFromGodfather(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            foreach (var godfather in grimoire.GetPlayersWithAbility(Character.Godfather).WithToken(Token.GodfatherKillsTonight))
            {
                var target = await godfather.Agent.RequestChoiceFromGodfather(grimoire.Players);
                storyteller.ChoiceFromGodfather(godfather, target);

                godfather.Tokens.Remove(Token.GodfatherKillsTonight);
                if (!godfather.DrunkOrPoisoned && target.Alive)
                {
                    await new Kills(storyteller, grimoire).NightKill(target, godfather);
                }
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
