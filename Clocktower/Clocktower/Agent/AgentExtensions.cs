using Clocktower.Game;
using Clocktower.Options;

namespace Clocktower.Agent
{
    internal static class AgentExtensions
    {
        public static async Task<Player> RequestChoiceFromImp(this IAgent agent, IEnumerable<Player> players)
        {
            return (await agent.RequestChoiceFromImp(players.ToOptions())).GetPlayer();
        }

        public static async Task<Player> RequestChoiceFromPoisoner(this IAgent agent, IEnumerable<Player> players)
        {
            return (await agent.RequestChoiceFromPoisoner(players.ToOptions())).GetPlayer();
        }

        public static async Task<Player?> RequestChoiceFromAssassin(this IAgent agent, IEnumerable<Player> players)
        {
            return (await agent.RequestChoiceFromAssassin(players.ToOptions().Prepend(new PassOption()).ToList())).GetPlayerOptional();
        }

        public static async Task<Player> RequestChoiceFromGodfather(this IAgent agent, IEnumerable<Player> players)
        {
            return (await agent.RequestChoiceFromGodfather(players.ToOptions())).GetPlayer();
        }

        public static async Task<Character?> RequestChoiceFromPhilosopher(this IAgent agent, IEnumerable<Character> characters)
        {
            return (await agent.RequestChoiceFromPhilosopher(characters.ToOptions().Prepend(new PassOption()).ToList())).GetCharacterOptional();
        }

        public static async Task<(Player targetA, Player targetB)> RequestChoiceFromFortuneTeller(this IAgent agent, IReadOnlyCollection<Player> players)
        {
            return (await agent.RequestChoiceFromFortuneTeller(players.ToTwoPlayersOptions())).GetTwoPlayers();
        }

        public static async Task<Player> RequestChoiceFromMonk(this IAgent agent, IEnumerable<Player> players)
        {
            return (await agent.RequestChoiceFromMonk(players.ToOptions())).GetPlayer();
        }

        public static async Task<Player> RequestChoiceFromRavenkeeper(this IAgent agent, IEnumerable<Player> players)
        {
            return (await agent.RequestChoiceFromRavenkeeper(players.ToOptions())).GetPlayer();
        }

        public static async Task<(Player? target, bool alwaysPass)> PromptSlayerShot(this IAgent agent, IEnumerable<Player> players, bool bluff)
        {
            var options = players.ToSlayerShotOptions(bluff).ToList();
            options.Insert(0, new PassOption());
            if (bluff)
            {
                options.Insert(0, new AlwaysPassOption());
            }
            return (await agent.PromptSlayerShot(options)).GetSlayerTargetOptional();
        }

        public static async Task<bool> PromptFishermanAdvice(this IAgent agent)
        {
            return await agent.PromptFishermanAdvice(OptionsBuilder.YesOrNo) is YesOption;
        }

        public static async Task<Player?> GetNomination(this IAgent agent, IEnumerable<Player> players)
        {
            return (await agent.GetNomination(players.ToOptions().Prepend(new PassOption()).ToList())).GetPlayerOptional();
        }

        public static async Task<bool> GetVote(this IAgent agent, Player nominee, bool ghostVote)
        {
            return await agent.GetVote(nominee.ToVoteOptions(), ghostVote) is VoteOption;
        }

        public static async Task<Player?> OfferPrivateChatOptional(this IAgent agent, IEnumerable<Player> players)
        {
            return (await agent.OfferPrivateChat(players.ToOptions().Prepend(new PassOption()).ToList())).GetPlayerOptional();
        }

        public static async Task<Player> OfferPrivateChatRequired(this IAgent agent, IEnumerable<Player> players)
        {
            return (await agent.OfferPrivateChat(players.ToOptions().ToList())).GetPlayer();
        }
    }
}
