using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Clocktower.Game
{
    /// <summary>
    /// Loads a script created by the Blood on the Clocktower script builder tool.
    /// https://script.bloodontheclocktower.com/
    /// </summary>
    internal static class ClocktowerScriptReader
    {
        public static IEnumerable<Character> ReadScriptFromFile(string fileName)
        {
            return ParseCharactersFromJson(ReadJsonFromFile(fileName));
        }

        private static object? ReadJsonFromFile(string fileName)
        {
            using StreamReader reader = new(fileName);
            using JsonTextReader jsonTextReader = new(reader);
            return new JsonSerializer().Deserialize(jsonTextReader);
        }

        private static IEnumerable<Character> ParseCharactersFromJson(object? json)
        {
            if (json is JArray array)
            {
                foreach (var item in array)
                {
                    if (item is JValue value)
                    {
                        var characterText = value.ToString();
                        if (!string.IsNullOrEmpty(characterText))
                        {
                            if (Enum.TryParse<Character>(characterText, ignoreCase: true, out var character))
                            {
                                yield return character;
                            }
                            else
                            {   // Script tool now strips out underscores from character names.
                                var matches = Enum.GetValues<Character>().Where(character => string.Equals(characterText, character.ToString().Replace("_", string.Empty), StringComparison.InvariantCultureIgnoreCase))
                                                                         .ToList();
                                if (matches.Any())
                                {
                                    yield return matches.First();
                                }
                                else
                                {
                                    throw new Exception($"Unknown character {characterText}");
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
