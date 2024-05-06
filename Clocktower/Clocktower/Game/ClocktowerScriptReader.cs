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
                    if (item is JObject jsonObject)
                    {
                        var character = jsonObject.GetValue("id")?.ToString();
                        if (!string.IsNullOrEmpty(character) && character != "_meta")
                        {
                            yield return Enum.Parse<Character>(character, ignoreCase: true);
                        }
                    }
                }
            }
        }
    }
}
