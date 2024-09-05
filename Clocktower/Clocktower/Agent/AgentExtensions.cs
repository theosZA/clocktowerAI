using Clocktower.Game;
using Clocktower.Options;

namespace Clocktower.Agent
{
    internal static class AgentExtensions
    {
        public static async Task<Character> RequestNewKazaliMinion(this IAgent agent, Player minionTarget, Character unavailableMinionCharacter, IEnumerable<Character> availableMinionCharacters)
        {
            return (await agent.RequestNewKazaliMinion(minionTarget, unavailableMinionCharacter, availableMinionCharacters.ToOptions())).GetCharacter();
        }

        public static async Task<Character> RequestChoiceOfMinionForSoldierSelectedByKazali(this IAgent agent, IEnumerable<Character> availableMinionCharacters)
        {
            return (await agent.RequestChoiceOfMinionForSoldierSelectedByKazali(availableMinionCharacters.ToOptions())).GetCharacter();
        }

        public static async Task<Player> RequestChoiceFromDemon(this IAgent agent, Character demonCharacter, IEnumerable<Player> players)
        {
            return (await agent.RequestChoiceFromDemon(demonCharacter, players.ToOptions())).GetPlayer();
        }

        public static async Task<Character> RequestChoiceFromOjo(this IAgent agent, IEnumerable<Character> characters)
        {
            return (await agent.RequestChoiceFromOjo(characters.ToOptions())).GetCharacter();
        }

        public static async Task<Player> RequestChoiceFromPoisoner(this IAgent agent, IEnumerable<Player> players)
        {
            return (await agent.RequestChoiceFromPoisoner(players.ToOptions())).GetPlayer();
        }

        public static async Task<Player> RequestChoiceFromWitch(this IAgent agent, IEnumerable<Player> players)
        {
            return (await agent.RequestChoiceFromWitch(players.ToOptions())).GetPlayer();
        }

        public static async Task<Player?> RequestChoiceFromAssassin(this IAgent agent, IEnumerable<Player> players)
        {
            return (await agent.RequestChoiceFromAssassin(players.ToOptions().Prepend(new PassOption()).ToList())).GetPlayerOptional();
        }

        public static async Task<Player> RequestChoiceFromGodfather(this IAgent agent, IEnumerable<Player> players)
        {
            return (await agent.RequestChoiceFromGodfather(players.ToOptions())).GetPlayer();
        }

        public static async Task<Player> RequestChoiceFromDevilsAdvocate(this IAgent agent, IEnumerable<Player> players)
        {
            return (await agent.RequestChoiceFromDevilsAdvocate(players.ToOptions())).GetPlayer();
        }

        public static async Task<Character?> RequestChoiceFromPhilosopher(this IAgent agent, IEnumerable<Character> characters)
        {
            return (await agent.RequestChoiceFromPhilosopher(characters.ToOptions().Prepend(new PassOption()).ToList())).GetCharacterOptional();
        }

        public static async Task<(Player targetA, Player targetB)> RequestChoiceFromFortuneTeller(this IAgent agent, IReadOnlyCollection<Player> players)
        {
            return (await agent.RequestChoiceFromFortuneTeller(players.ToTwoPlayersOptions())).GetTwoPlayers();
        }

        public static async Task<Player?> RequestChoiceFromNightwatchman(this IAgent agent, IEnumerable<Player> players)
        {
            return (await agent.RequestChoiceFromNightwatchman(players.ToOptions().Prepend(new PassOption()).ToList())).GetPlayerOptional();
        }

        public static async Task<Player> RequestChoiceFromMonk(this IAgent agent, IEnumerable<Player> players)
        {
            return (await agent.RequestChoiceFromMonk(players.ToOptions())).GetPlayer();
        }

        public static async Task<Player> RequestChoiceFromRavenkeeper(this IAgent agent, IEnumerable<Player> players)
        {
            return (await agent.RequestChoiceFromRavenkeeper(players.ToOptions())).GetPlayer();
        }

        public static async Task<Player> RequestChoiceFromButler(this IAgent agent, IEnumerable<Player> players)
        {
            return (await agent.RequestChoiceFromButler(players.ToOptions())).GetPlayer();
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
