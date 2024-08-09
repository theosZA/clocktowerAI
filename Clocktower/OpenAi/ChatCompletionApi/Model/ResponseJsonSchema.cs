using System.Text.Json.Serialization;

namespace OpenAi.ChatCompletionApi.Model
{
    internal class ResponseJsonSchema
    {
        public string Name { get; set; } = string.Empty;

        public bool Strict { get; set; } = true;

        [JsonConverter(typeof(RawJsonConverter))]
        public string? Schema { get; set; }
    }
}
