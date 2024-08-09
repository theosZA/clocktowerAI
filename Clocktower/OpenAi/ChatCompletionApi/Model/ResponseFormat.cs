using NJsonSchema;
using NJsonSchema.Generation;
using System.Text.Json.Serialization;

namespace OpenAi.ChatCompletionApi.Model
{
    /// <summary>
    /// An object specifying the format that the model must output. 
    /// Setting Type to "json_schema" and JsonSchema to a JSON schema enables Structured Outputs which ensures the model will match your supplied JSON schema. 
    /// </summary>
    internal class ResponseFormat
    {
        public static ResponseFormat ResponseFormatFromType<T>()
        {
            var settings = new SystemTextJsonSchemaGeneratorSettings()
            {
                GenerateEnumMappingDescription = true
            };
            
            return new ResponseFormat
            {
                Type = "json_schema",
                ResponseJsonSchema = new()
                {
                    Name = typeof(T).Name,
                    Schema = JsonSchema.FromType<T>(settings).ToJson()
                }
            };
        }

        public string? Type { get; set; }

        [JsonPropertyName("json_schema")]
        public ResponseJsonSchema? ResponseJsonSchema { get; set; }
    }
}
