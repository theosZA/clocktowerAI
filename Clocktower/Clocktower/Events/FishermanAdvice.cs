using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class FishermanAdvice : IGameEvent
    {
        /// <summary>
        /// Current day nominations - only set if we want to restrict the Fisherman advice option to the player who is about to be executed.
        /// </summary>
        public Nominations? Nominations { get; set; }

        public FishermanAdvice(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            foreach (var fisherman in GetPlayersWhoCanAskForFishermanAdvice())
            {
                if (await fisherman.Agent.PromptFishermanAdvice())
                {
                    fisherman.Tokens.Add(Token.UsedOncePerGameAbility);
                    var advice = await storyteller.GetFishermanAdvice(fisherman);
                    fisherman.Agent.ResponseForFisherman(advice);
                }
            }   
        }

        private IEnumerable<Player> GetPlayersWhoCanAskForFishermanAdvice()
        {
            if (Nominations != null)
            {   // We only allow the player about to be executed to get their Fisherman advice here.
                if (Nominations.PlayerToBeExecuted != null && CanPlayerAskForFishermanAdvice(Nominations.PlayerToBeExecuted))
                {
                    return new[] { Nominations.PlayerToBeExecuted };
                }
                return Array.Empty<Player>();
            }
            return grimoire.Players.Where(CanPlayerAskForFishermanAdvice).ToList();
        }

        private static bool CanPlayerAskForFishermanAdvice(Player player)
        {
            return player.Character == Character.Fisherman && player.Alive && !player.Tokens.Contains(Token.UsedOncePerGameAbility);
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
