using Clocktower.Game;
using Clocktower.Options;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Clocktower.Agent.RobotAgent.Model
{
    /// <summary>
    /// AI model for specifying any number of characters.
    /// </summary>
    internal class CharactersSelection : IOptionSelection
    {
        [Required(AllowEmptyStrings = true)]
        public string Reasoning { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = true)]
        public IEnumerable<string> Characters { get; set; } = Enumerable.Empty<string>();

        public IOption? PickOption(IReadOnlyCollection<IOption> options)
        {
            var characters = Characters.ToList();
            return options.FirstOrDefault(option => option is ThreeCharactersOption threeCharactersOption && IsMatchingOption(threeCharactersOption, characters));
        }

        public string NoMatchingOptionPrompt(IReadOnlyCollection<IOption> options)
        {
            var sb = new StringBuilder();

            // First check to see if they've used any invalid characters. This is the more likely case and helps us avoid listing all the possible arrays.
            var allPossibleCharacters = GetAllPossibleCharacters(options).ToList();
            var invalidCharacters = Characters.Except(allPossibleCharacters).ToList();
            if (invalidCharacters.Count > 0)
            {
                sb.AppendLine($"The following characters are not valid choices: {string.Join(", ", invalidCharacters)}. The characters in the array must come from the following list:");
                foreach (var character in allPossibleCharacters)
                {
                    sb.AppendLine($"- {character}");
                }
            }
            else
            {
                sb.AppendLine($"'{string.Join(",", Characters)}' is not a valid choice. `{nameof(Characters)}` property must be one of the following arrays:");
                foreach (var option in options)
                {
                    if (option is ThreeCharactersOption threeCharactersOption)
                    {
                        sb.AppendLine($"- [\"{TextUtilities.CharacterToText(threeCharactersOption.CharacterA)}\",\"{TextUtilities.CharacterToText(threeCharactersOption.CharacterB)}\",\"{TextUtilities.CharacterToText(threeCharactersOption.CharacterC)}\"]");
                    }
                }
            }

            return sb.ToString();
        }

        private static bool IsMatchingOption(ThreeCharactersOption option, List<string> characters)
        {
            if (characters.Count != 3)
            {
                return false;
            }
            return characters.Contains(TextUtilities.CharacterToText(option.CharacterA))
                && characters.Contains(TextUtilities.CharacterToText(option.CharacterB))
                && characters.Contains(TextUtilities.CharacterToText(option.CharacterC));
        }

        private static IEnumerable<string> GetAllPossibleCharacters(IReadOnlyCollection<IOption> options)
        {
            var allPossibleCharacters = new HashSet<Character>();
            foreach (var option in options)
            {
                if (option is ThreeCharactersOption threeCharactersOption)
                {
                    allPossibleCharacters.Add(threeCharactersOption.CharacterA);
                    allPossibleCharacters.Add(threeCharactersOption.CharacterB);
                    allPossibleCharacters.Add(threeCharactersOption.CharacterC);
                }
            }
            return allPossibleCharacters.Select(TextUtilities.CharacterToText);
        }
    }
}
