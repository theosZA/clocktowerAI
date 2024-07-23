using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class FishermanAdvice : IGameEvent
    {
        public FishermanAdvice(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            foreach (var fisherman in grimoire.Players.Where(player => player.ShouldRunAbility(Character.Fisherman) && !player.Tokens.HasToken(Token.UsedOncePerGameAbility)))
            {
                if (await fisherman.Agent.PromptFishermanAdvice())
                {
                    fisherman.Tokens.Add(Token.UsedOncePerGameAbility, fisherman);
                    var advice = await storyteller.GetFishermanAdvice(fisherman);
                    await fisherman.Agent.ResponseForFisherman(advice);
                }
            }   
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
