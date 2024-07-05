using Clocktower.Game;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

namespace Clocktower.Agent
{
    /// <summary>
    /// Privates methods for building text content that can be sent to players for describing the game.
    /// </summary>
    internal static class TextBuilder
    {
        public static string ScriptToText(string scriptName, IReadOnlyCollection<Character> script)
        {
            var characterDescriptions = ReadCharacterDescriptionsFromFile("Scripts\\Characters.txt");

            var sb = new StringBuilder();
            sb.AppendLine($"This game will use a script called '{scriptName}' which includes the following characters:");
            sb.AppendLine("Townsfolk (good):");
            foreach (var townsfolk in script.Where(character => character.CharacterType() == CharacterType.Townsfolk))
            {
                sb.AppendLine(CharacterToText(townsfolk, characterDescriptions));
            }
            sb.AppendLine("Outsider (good):");
            foreach (var outsider in script.Where(character => character.CharacterType() == CharacterType.Outsider))
            {
                sb.AppendLine(CharacterToText(outsider, characterDescriptions));
            }
            sb.AppendLine("Minion (evil):");
            foreach (var minion in script.Where(character => character.CharacterType() == CharacterType.Minion))
            {
                sb.AppendLine(CharacterToText(minion, characterDescriptions));
            }
            sb.AppendLine("Demon (evil):");
            foreach (var demon in script.Where(character => character.CharacterType() == CharacterType.Demon))
            {
                sb.AppendLine(CharacterToText(demon, characterDescriptions));
            }
            return sb.ToString();
        }

        public static string SetupToText(int playerCount)
        {
            return $"In this game there are {playerCount} players. That means there will be {TownsfolkCount(playerCount)} Townsfolk, {OutsiderCount(playerCount)} Outsiders, {MinionCount(playerCount)} Minions and 1 Demon (unless modified by a Godfather).\r\n";
        }

        public static string PlayersToText(IReadOnlyCollection<string> playerNames)
        {
            var sb = new StringBuilder();

            sb.Append("In this game we have the following players, going clockwise around town: ");
            foreach (var name in playerNames.SkipLast(1))
            {
                sb.Append(name);
                sb.Append(", ");
            }
            sb.Append(playerNames.Last());
            sb.AppendLine(". ");

            return sb.ToString();
        }

        private static string CharacterToText(Character character, IDictionary<Character, string> characterDescriptions)
        {
            if (!characterDescriptions.TryGetValue(character, out var description))
            {
                throw new InvalidEnumArgumentException(nameof(character));
            }
            return $"- {TextUtilities.CharacterToText(character)}: {description}";
        }

        private static IDictionary<Character, string> ReadCharacterDescriptionsFromFile(string fileName)
        {
            var characterDescriptions = new Dictionary<Character, string>();
            string[] lines = File.ReadAllLines(fileName);
            foreach (string line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("//"))
                {
                    Match match = descriptionRegex.Match(line);
                    if (match.Success)
                    {
                        string characterName = match.Groups[1].Value.Trim().Replace(" ", "_");
                        string description = match.Groups[2].Value.Trim();

                        if (!Enum.TryParse(characterName, ignoreCase: true, out Character character))
                        {
                            throw new Exception($"Unknown character {characterName} in descriptions file \"{fileName}\"");
                        }
                        characterDescriptions[character] = description;
                    }
                }
            }
            return characterDescriptions;
        }

        private static int TownsfolkCount(int playerCount)
        {
            return playerCount - (OutsiderCount(playerCount) + MinionCount(playerCount) + 1);
        }

        private static int OutsiderCount(int playerCount)
        {
            return (playerCount + 2) % 3;
        }

        private static int MinionCount(int playerCount)
        {
            return (playerCount - 4) / 3;
        }

        private static readonly Regex descriptionRegex = new(@"^([\w\s]+):(.+)$");
    }
}
