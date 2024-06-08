using Clocktower.Game;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

namespace Clocktower.Agent.RobotAgent
{
    internal static class SystemMessage
    {
        public static string GetSystemMessage(string playerName, string personality, IReadOnlyCollection<string> playerNames, IReadOnlyCollection<Character> script)
        {
            return $"We are going to play a game of 'Blood on the Clocktower'. This is a social deduction game where the players are divided into good Townsfolk and Outsiders and evil Minions and Demons. " +
            "The evil players know who is on which side and they win when there are only two players left alive, one of which is their Demon. " +
            "The good players don't know who is on which side and must use their character abilities and social skills to determine who the evil players are. The good players win if the Demon dies, usually by executing them. " +
            "The game is split into two phases: a day phase and a night phase.\r\n" +
            "During the day, you talk. You will each have a secret identity, a unique character from the script. Generally, the good players share whatever they know and attempt to find out who is who. " +
            "Most good players will be telling the truth, but some have an incentive to lie. If you are evil, you should definitely be lying (except in private conversations with your fellow evil players)! " +
            "It is best to pick a good character to pretend to be, spreading as much false information as possible. " +
            "Each day you will get a chance to make public statements and have private conversations. Early private conversations are often used to exchange information about what character you are and possibly what information you learned. " +
            "A common tactic is to do a \"2 for 2\", or \"3 for 3\", in which each player gives 2 or 3 characters which is supposed to include their real character, in exchange for the same from the other player. " +
            "There's no need to encourage teamwork and co-operation with your fellow players - that's what everybody should be doing anyway (or at least bluffing that's what they're doing). " +
            "Instead you are encouraged to be proactive in sharing information and working out plans together.\r\n" +
            "At the end of each day there will be a chance to nominate players for execution. Only one player may be executed each day, and the good team usually wants to execute a player each day since executing the Demon is how they win. " +
            "Each player only gets one nomination each day, and the same player can't be nominated more than once in a day. However, each day you may vote for as few or as many players as you wish, and whoever has the most votes is executed. " +
            "This player needs a vote tally of at least 50% of the living players or no execution occurs. On a tie, neither player is executed.\r\n" +
            "During the night, some players will get a chance to secretly use their ability or gain some type of information. This includes the Demon who can kill one player every night after the first. " +
            "Most of you will die - but death is not the end. Some players may even want to die, as they gain information when they do. If you are dead, you still participate in the game, you may still talk, " +
            "and you still win or lose with your team. In fact, the game is usually decided by the votes and opinions of the dead players. " +
            "When you die, you lose your character ability, you may no longer nominate, and you have only one vote for the rest of the game, so use it wisely.\r\n" +
            "There is a lot of information in this game. However, some of it might be wrong. If you are drunk or poisoned, you have no ability, but I will pretend that you do: " +
            "your ability won't work and any information you get from your ability may be incorrect.\r\n" +
            "This game will use a script called 'A Simple Matter' which includes the following characters:\r\n" +
            ScriptToText(script) +
            SetupToText(playerNames.Count) +
            PlayersToText(playerNames) +
            $"You are {playerName}, a player in the game. (Your clockwise neighbour is {ClockwiseNeighbour(playerNames, playerName)} and your anti-clockwise neighbour is {AnticlockwiseNeighbour(playerNames, playerName)}.) " +
            "{personality} Try to bring out your personality in your conversations with other players.\r\n";
        }

        private static string ScriptToText(IReadOnlyCollection<Character> script)
        {
            var characterDescriptions = ReadCharacterDescriptionsFromFile("Scripts\\Characters.txt");

            var sb = new StringBuilder();

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

        private static string CharacterToText(Character character, IDictionary<Character, string> characterDescriptions)
        {
            if (!characterDescriptions.TryGetValue(character, out var description))
            {
                throw new InvalidEnumArgumentException(nameof(character));
            }
            return $"- {TextUtilities.CharacterToText(character)}: {description}";
        }

        private static string SetupToText(int playerCount)
        {
            return $"In this game there are {playerCount} players. That means there will be {TownsfolkCount(playerCount)} Townsfolk, {OutsiderCount(playerCount)} Outsiders, {MinionCount(playerCount)} Minions and 1 Demon (unless modified by a Godfather).\r\n";
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

        private static string PlayersToText(IReadOnlyCollection<string> playerNames)
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

        private static string ClockwiseNeighbour(IReadOnlyCollection<string> playerNames, string fromPlayer)
        {
            var playersList = playerNames.ToList();

            int fromPlayerIndex = playersList.IndexOf(fromPlayer);
            if (fromPlayerIndex == -1)
            {
                throw new ArgumentException("The player is not in the list of player names", nameof(fromPlayer));
            }

            int clockwiseNeighbourIndex = (fromPlayerIndex + 1) % playersList.Count;
            return playersList[clockwiseNeighbourIndex];
        }

        private static string AnticlockwiseNeighbour(IReadOnlyCollection<string> playerNames, string fromPlayer)
        {
            var playersList = playerNames.ToList();

            int fromPlayerIndex = playersList.IndexOf(fromPlayer);
            if (fromPlayerIndex == -1)
            {
                throw new ArgumentException("The player is not in the list of player names", nameof(fromPlayer));
            }

            int clockwiseNeighbourIndex = (fromPlayerIndex + playersList.Count - 1) % playersList.Count;
            return playersList[clockwiseNeighbourIndex];
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

        private static readonly Regex descriptionRegex = new(@"^([\w\s]+):(.+)$");
    }
}
