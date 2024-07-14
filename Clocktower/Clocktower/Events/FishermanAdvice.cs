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
                    fisherman.Tokens.Add(Token.UsedOncePerGameAbility, fisherman);
                    var advice = await storyteller.GetFishermanAdvice(fisherman);
                    await fisherman.Agent.ResponseForFisherman(advice);
                }
            }   
        }

        private IEnumerable<Player> GetPlayersWhoCanAskForFishermanAdvice()
        {
            if (grimoire.PlayerToBeExecuted == null)
            {
                return grimoire.Players.Where(CanPlayerAskForFishermanAdvice).ToList();
            }

            // Ending day with a player about to be executed. Only they are allowed to still ask for Fisherman advice here.
            if (CanPlayerAskForFishermanAdvice(grimoire.PlayerToBeExecuted))
            {
                return new List<Player> { grimoire.PlayerToBeExecuted };
            }

            return new List<Player>();
        }

        private static bool CanPlayerAskForFishermanAdvice(Player player)
        {
            return player.Character == Character.Fisherman && player.Alive && !player.Tokens.HasToken(Token.UsedOncePerGameAbility);
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
