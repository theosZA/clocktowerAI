using System.Text.Json.Serialization;

namespace OpenAi.ChatCompletionApi.Model
{
    internal class Usage
    {
        /// <summary>
        /// Number of tokens in the generated completion.
        /// </summary>
        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; } = 0;

        /// <summary>
        /// Number of tokens in the prompt.
        /// </summary>
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; } = 0;

        /// <summary>
        /// Total number of tokens used in the request (prompt + completion).
        /// </summary>
        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; } = 0;
    }
}
