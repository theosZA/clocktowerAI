using Clocktower.Game;
using Clocktower.Options;

namespace Clocktower.Storyteller
{
    internal static class StorytellerExtensions
    {
        public static async Task<IEnumerable<Character>> GetDemonBluffs(this IStoryteller storyteller, Player demon, IReadOnlyCollection<Character> availableBluffs)
        {
            return (await storyteller.GetDemonBluffs(demon, availableBluffs.ToThreeCharactersOptions())).GetThreeCharacters();
        }

        public static async Task<IEnumerable<Character>> GetMinionBluffs(this IStoryteller storyteller, Player minion, IReadOnlyCollection<Character> availableBluffs)
        {
            return (await storyteller.GetMinionBluffs(minion, availableBluffs.ToThreeCharactersOptions())).GetThreeCharacters();
        }

        public static async Task<Player> GetNewImp(this IStoryteller storyteller, IEnumerable<Player> impCandidates)
        {
            return (await storyteller.GetNewImp(impCandidates.ToOptions())).GetPlayer();
        }

        public static async Task<IEnumerable<Player>> GetOjoVictims(this IStoryteller storyteller, Player ojo, Character targetCharacter, IEnumerable<Player> livingPlayers)
        {
            return (await storyteller.GetOjoVictims(ojo, targetCharacter, livingPlayers.ToList().ToAllPossibleSubsetsAsOptions())).GetPlayers();
        }

        public static async Task<Player> GetOjoVictim(this IStoryteller storyteller, Player ojo, Character targetCharacter, IEnumerable<Player> matchingPlayers)
        {   // This is the case for when the Ojo picks a character and there are multiple such players with that character currently alive.
            return (await storyteller.GetOjoVictims(ojo, targetCharacter, matchingPlayers.ToOptions())).GetPlayer();
        }

        public static async Task<Player> GetDrunk(this IStoryteller storyteller, IEnumerable<Player> drunkCandidates)
        {
            return (await storyteller.GetDrunk(drunkCandidates.ToOptions())).GetPlayer();
        }

        public static async Task<Player> GetSweetheartDrunk(this IStoryteller storyteller, IEnumerable<Player> drunkCandidates)
        {
            return (await storyteller.GetSweetheartDrunk(drunkCandidates.ToOptions())).GetPlayer();
        }

        public static async Task<Player> GetFortuneTellerRedHerring(this IStoryteller storyteller, Player fortuneTeller, IEnumerable<Player> redHerringCandidates)
        {
            return (await storyteller.GetFortuneTellerRedHerring(fortuneTeller, redHerringCandidates.ToOptions())).GetPlayer();
        }

        public static async Task<Player> GetStewardPing(this IStoryteller storyteller, Player steward, IEnumerable<Player> stewardPingCandidates)
        {
            return (await storyteller.GetStewardPing(steward, stewardPingCandidates.ToOptions())).GetPlayer();
        }

        public static async Task<int> GetChefNumber(this IStoryteller storyteller, Player chef, IEnumerable<Player> playersThatCanMisregister, IEnumerable<int> chefOptions)
        {
            return (await storyteller.GetChefNumber(chef, playersThatCanMisregister, chefOptions.ToOptions())).GetNumber();
        }

        public static async Task<int> GetEmpathNumber(this IStoryteller storyteller, Player empath, Player neighbourA, Player neighbourB, IEnumerable<int> empathOptions)
        {
            return (await storyteller.GetEmpathNumber(empath, neighbourA, neighbourB, empathOptions.ToOptions())).GetNumber();
        }

        public static async Task<bool> GetFortuneTellerReading(this IStoryteller storyteller, Player fortuneTeller, Player targetA, Player targetB)
        {
            return await storyteller.GetFortuneTellerReading(fortuneTeller, targetA, targetB, OptionsBuilder.YesOrNo) is YesOption;
        }

        public static async Task<Direction> GetShugenjaDirection(this IStoryteller storyteller, Player shugenja, Grimoire grimoire)
        {
            return (await storyteller.GetShugenjaDirection(shugenja, grimoire, OptionsBuilder.DirectionOptions)).GetDirection();
        }

        public static async Task<Character> GetCharacterForRavenkeeper(this IStoryteller storyteller, Player ravenkeeper, Player target, IEnumerable<Character> ravenkeeperOptions)
        {
            return (await storyteller.GetCharacterForRavenkeeper(ravenkeeper, target, ravenkeeperOptions.ToOptions())).GetCharacter();
        }

        public static async Task<Character> GetCharacterForUndertaker(this IStoryteller storyteller, Player undertaker, Player executedPlayer, IEnumerable<Character> undertakerOptions)
        {
            return (await storyteller.GetCharacterForUndertaker(undertaker, executedPlayer, undertakerOptions.ToOptions())).GetCharacter();
        }

        public static async Task<Player> GetMayorBounce(this IStoryteller storyteller, Player mayor, Player? killer, IEnumerable<Player> mayorOptions)
        {
            return (await storyteller.GetMayorBounce(mayor, killer, mayorOptions.ToOptions())).GetPlayer();
        }

        public static async Task<bool> ShouldKillTinker(this IStoryteller storyteller, Player tinker)
        {
            return await storyteller.ShouldKillTinker(tinker, OptionsBuilder.YesOrNo) is YesOption;
        }

        public static async Task<bool> ShouldKillWithSlayer(this IStoryteller storyteller, Player slayer, Player target)
        {
            return await storyteller.ShouldKillWithSlayer(slayer, target, OptionsBuilder.YesOrNo) is YesOption;
        }

        public static async Task<bool> ShouldExecuteWithVirgin(this IStoryteller storyteller, Player virgin, Player nominator)
        {
            return await storyteller.ShouldExecuteWithVirgin(virgin, nominator, OptionsBuilder.YesOrNo) is YesOption;
        }
    }
}
