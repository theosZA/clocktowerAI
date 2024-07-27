using Clocktower.Game;
using Clocktower.Options;
using static System.Windows.Forms.Design.AxImporter;

namespace Clocktower.Agent
{
    /// <summary>
    /// Provides utility methods for reading Clocktower items from player- or AI-provided text.
    /// </summary>
    internal static class TextParser
    {
        public static Player? ReadPlayerFromText(string text, IReadOnlyCollection<Player> possiblePlayers)
        {
            text = text.Trim();

            // Look for exact player name match first.
            foreach (var player in possiblePlayers)
            {
                if (text.Equals(player.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return player;
                }
            }

            // Then just see if there's a match somewhere in the text.
            foreach (var player in possiblePlayers)
            {
                if (text.Contains(player.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return player;
                }
            }

            // No matches.
            return null;
        }

        public static Character? ReadCharacterFromText(string text, IReadOnlyCollection<Character> scriptCharacters)
        {
            text = text.Trim();

            // Look for exact character match first.
            foreach (var character in scriptCharacters)
            {
                if (text.Equals(TextUtilities.CharacterToText(character), StringComparison.InvariantCultureIgnoreCase))
                {
                    return character;
                }
            }

            // Then just see if there's a match somewhere in the text.
            foreach (var character in scriptCharacters)
            {
                if (text.Contains(TextUtilities.CharacterToText(character), StringComparison.InvariantCultureIgnoreCase))
                {
                    return character;
                }
            }

            // No matches.
            return null;
        }

        public static IOption ReadVoteOptionFromText(string text, IReadOnlyCollection<IOption> options)
        {
            var passOption = CleanUpText(ref text, options);
            if (passOption != null)
            {
                return passOption;
            }

            // Whichever of the two words "pass" or "execute" appears later in the text is the option we'll go with.
            int passPos = text.LastIndexOf("pass", StringComparison.InvariantCultureIgnoreCase);
            int executePos = text.LastIndexOf("execute", StringComparison.InvariantCultureIgnoreCase);
            return executePos <= passPos ? options.First(option => option is PassOption)
                                         : options.First(option => option is VoteOption);
        }

        public static IOption ReadShenaniganOptionFromText(string text, IReadOnlyCollection<IOption> options)
        {
            var passOption = CleanUpText(ref text, options);
            if (passOption != null)
            {
                return passOption;
            }

            // For each of our options, we're going to score it based on how closely the AI response matches what it's looking for.
            return options.Select(option => (option, Score(option, text))).MaxBy(pair => pair.Item2).option;
        }

        private static IOption? CleanUpText(ref string text, IReadOnlyCollection<IOption> options)
        {
            text = text.Trim();

            if (string.IsNullOrEmpty(text))
            {
                return options.First(option => option is PassOption);   // There should always be a Pass option available here.
            }

            // If the response includes quote marks, we assume the actual choice is within the quote marks.
            var quotedText = text.TextBetween('"', '"').Trim();
            if (!string.IsNullOrEmpty(quotedText))
            {
                text = quotedText;
            }

            // Remove trailing punctuation.
            if (text.EndsWith('.') || text.EndsWith('!'))
            {
                text = text[..^1];
            }

            return null;
        }

        private static int Score(IOption option, string text)
        {
            if (option is AlwaysPassOption)
            {
                if (text.Equals("always pass", StringComparison.InvariantCultureIgnoreCase))
                {
                    return 100;
                }
                if (text.EndsWith("always pass", StringComparison.InvariantCultureIgnoreCase))
                {
                    return 90;
                }
                if (text.Contains("always pass", StringComparison.InvariantCultureIgnoreCase))
                {
                    return 50;
                }
                if (text.Contains("always", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (text.Contains("pass", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return 25;
                    }
                    return 10;
                }
                return 0;
            }
            if (option is PassOption)
            {
                if (text.Equals("pass", StringComparison.InvariantCultureIgnoreCase))
                {
                    return 100;
                }
                if (text.EndsWith("pass", StringComparison.InvariantCultureIgnoreCase))
                {
                    return 80;
                }
                if (text.Contains("pass", StringComparison.InvariantCultureIgnoreCase))
                {
                    return 40;
                }
                return 1;   // Even if this doesn't match anything else, we'd prefer to have a Pass here rather than another random option that doesn't match.
            }
            if (option is SlayerShotOption slayerShotOption)
            {
                // Ideally we should be able to find the text "Slayer:" in our string to identify that this is an attempted Slayer shot.
                int slayerScore = 0;
                int? targetPos = null;
                if (text.Contains("Slayer:", StringComparison.InvariantCultureIgnoreCase))
                {
                    slayerScore = 90;
                    targetPos = text.IndexOf("Slayer:", StringComparison.InvariantCultureIgnoreCase) + 7;
                }
                else if (text.Contains("Slayer", StringComparison.InvariantCultureIgnoreCase))
                {
                    slayerScore = 10;   // Not a good match because a Juggler might easily juggle someone as the Slayer.
                    targetPos = text.IndexOf("Slayer", StringComparison.InvariantCultureIgnoreCase) + 6;
                }
                if (targetPos.HasValue)
                {
                    var target = text[targetPos.Value..];
                    slayerShotOption.SetTargetFromText(target);
                    if (slayerShotOption != null)
                    {
                        return slayerScore;
                    }
                }
                return 0;
            }
            else if (option is JugglerOption jugglerOption)
            {
                // Ideally we should be able to find the text "Juggler:" in our string to identify that this is an attempted Juggler juggle.
                int jugglerScore = 0;
                int? jugglePos = null;
                if (text.Contains("Juggler:", StringComparison.InvariantCultureIgnoreCase))
                {
                    jugglerScore = 95;
                    jugglePos = text.IndexOf("Juggler:", StringComparison.InvariantCultureIgnoreCase) + 8;
                }
                else if (text.Contains("Juggler", StringComparison.InvariantCultureIgnoreCase))
                {
                    jugglerScore = 10;   // Not a good match because a Juggler might easily juggle someone as the Slayer.
                    jugglePos = text.IndexOf("Juggler", StringComparison.InvariantCultureIgnoreCase) + 7;
                }
                if (jugglePos.HasValue)
                {
                    var juggleText = text[jugglePos.Value..];
                    if (jugglerOption.AddJugglesFromText(juggleText))
                    {
                        return jugglerScore;
                    }
                }
                return 0;
            }

            // The option was not an option that we know how to score.
            return 0;
        }
    }
}
