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

            sb.AppendLine($"# {scriptName}");
            sb.AppendLine("The following characters are available in this game...");

            sb.AppendLine(CharacterTypeHeadingToText(CharacterType.Townsfolk, Alignment.Good));
            foreach (var townsfolk in script.Where(character => character.CharacterType() == CharacterType.Townsfolk))
            {
                sb.AppendLine(CharacterToText(townsfolk, characterDescriptions));
            }
            sb.AppendLine(CharacterTypeHeadingToText(CharacterType.Outsider, Alignment.Good));
            foreach (var outsider in script.Where(character => character.CharacterType() == CharacterType.Outsider))
            {
                sb.AppendLine(CharacterToText(outsider, characterDescriptions));
            }
            sb.AppendLine(CharacterTypeHeadingToText(CharacterType.Minion, Alignment.Evil));
            foreach (var minion in script.Where(character => character.CharacterType() == CharacterType.Minion))
            {
                sb.AppendLine(CharacterToText(minion, characterDescriptions));
            }
            sb.AppendLine(CharacterTypeHeadingToText(CharacterType.Demon, Alignment.Evil));
            foreach (var demon in script.Where(character => character.CharacterType() == CharacterType.Demon))
            {
                sb.AppendLine(CharacterToText(demon, characterDescriptions));
            }

            var jinxes = ReadJinxesFromFile("Scripts\\Jinxes.txt").Where(jinx => script.Contains(jinx.character1) && script.Contains(jinx.character2)).ToList();
            if (jinxes.Any())
            {
                sb.AppendLine("## Jinxes");
                sb.AppendLine("There are special rules that govern how the following pairs of characters interact when they are both on the same script.");
                foreach (var jinx in jinxes)
                {
                    sb.AppendFormattedText($"- %c / %c: {jinx.jinx}", jinx.character1, jinx.character2);
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }

        public static string SetupToText(int playerCount, IReadOnlyCollection<Character> script)
        {
            var sb = new StringBuilder();
            sb.Append($"In this game there are {playerCount} players. That means there will be {CharacterTypeCountToText(CharacterType.Townsfolk, playerCount)}, {CharacterTypeCountToText(CharacterType.Outsider, playerCount)}, {CharacterTypeCountToText(CharacterType.Minion, playerCount)} and {CharacterTypeCountToText(CharacterType.Demon, playerCount)}");

            var charactersThatModifySetup = GetCharactersThatCanAlterSetupCounts(script).ToList();
            if (charactersThatModifySetup.Count > 0)
            {
                sb.Append($" (unless modified by a {charactersThatModifySetup[0]}");
                if (charactersThatModifySetup.Count > 1)
                {
                    for (int i = 1; i < charactersThatModifySetup.Count - 1; ++i)
                    {
                        sb.Append($", {charactersThatModifySetup[i]}");
                    }
                    sb.Append($" or {charactersThatModifySetup.Last()}");
                }
                sb.Append(')');
            }
            sb.AppendLine();

            return sb.ToString();
        }

        public static string PlayersToText(IReadOnlyCollection<string> playerNames)
        {
            return ($"In this game we have the following players, going clockwise around town: {string.Join(", ", playerNames.Select(name => $"**{name}**"))}.\n");
        }

        public static string GrimoireToText(Grimoire grimoire)
        {
            var sb = new StringBuilder();

            foreach (var player in grimoire.Players)
            {
                var aliveStatus = player.Tokens.HasToken(Token.DiedAtNight) ? "Died tonight"
                                                             : player.Alive ? "Alive" 
                                                                            : "Dead";
                sb.AppendFormattedText($"- %p - {aliveStatus} - %a - %c - {player.Tokens}", player, player.Alignment, player.Character);
                sb.AppendLine();
            }

            sb.AppendFormattedText("The characters not in play shown to the demon: %C", grimoire.DemonBluffs);

            return sb.ToString();
        }

        private static string CharacterTypeHeadingToText(CharacterType characterType, Alignment alignment)
        {
            return $"## {characterType} ({alignment}):";
        }

        private static string CharacterTypeCountToText(CharacterType characterType, int playerCount)
        {
            var count = characterType switch
            {
                CharacterType.Townsfolk => TownsfolkCount(playerCount),
                CharacterType.Outsider => OutsiderCount(playerCount),
                CharacterType.Minion => MinionCount(playerCount),
                CharacterType.Demon => 1,
                _ => throw new InvalidEnumArgumentException(nameof(characterType), (int)characterType, typeof(CharacterType))
            };

            var name = characterType.ToString();
            if (count != 1 && characterType != CharacterType.Townsfolk)
            {   // All character types take an 's' to make plural except townsfolk.
                name += 's';
            }

            return $"{count} {name}";
        }

        private static string CharacterToText(Character character, IDictionary<Character, string> characterDescriptions)
        {
            if (!characterDescriptions.TryGetValue(character, out var description))
            {
                throw new InvalidEnumArgumentException(nameof(character));
            }
            return $"- **{TextUtilities.CharacterToText(character)}**: {description}";
        }

        private static IDictionary<Character, string> ReadCharacterDescriptionsFromFile(string fileName)
        {
            var characterDescriptions = new Dictionary<Character, string>();
            var lines = File.ReadAllLines(fileName);
            foreach (var line in lines)
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

        private static IReadOnlyCollection<(Character character1, Character character2, string jinx)> ReadJinxesFromFile(string fileName)
        {
            var jinxes = new List<(Character, Character, string)>();
            var lines = File.ReadAllLines(fileName);
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("//"))
                {
                    Match match = jinxRegex.Match(line);
                    if (match.Success)
                    {
                        string characterName1 = match.Groups[1].Value.Trim().Replace(" ", "_");
                        string characterName2 = match.Groups[2].Value.Trim().Replace(" ", "_");
                        string description = match.Groups[3].Value.Trim();

                        if (!Enum.TryParse(characterName1, out Character character1))
                        {
                            throw new Exception($"Unknown character {characterName1} in jinxes file \"{fileName}\"");

                        }
                        if (!Enum.TryParse(characterName2, out Character character2))
                        {
                            throw new Exception($"Unknown character {characterName2} in jinxes file \"{fileName}\"");
                        }
                        jinxes.Add((character1, character2, description));
                    }
                }
            }
            return jinxes;
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

        private static IEnumerable<Character> GetCharactersThatCanAlterSetupCounts(IReadOnlyCollection<Character> script)
        {
            return script.Where(character => character == Character.Godfather
                                          || character == Character.Baron);
        }

        private static readonly Regex descriptionRegex = new(@"^([\w\s]+):(.+)$");
        private static readonly Regex jinxRegex = new(@"^([\w\s]+) / ([\w\s]+):(.+)$");
    }
}
