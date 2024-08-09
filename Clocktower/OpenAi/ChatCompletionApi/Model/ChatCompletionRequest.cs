using System.Text.Json.Serialization;

namespace OpenAi.ChatCompletionApi.Model
{
    internal class ChatCompletionRequest
    {
        /// <summary>
        /// ID of the model to use. See the model endpoint compatibility table (https://platform.openai.com/docs/models/model-endpoint-compatibility)
        /// for details on which models work with the Chat API.
        /// </summary>
        public string Model { get; set; } = "gpt-4o-mini";

        /// <summary>
        /// A list of messages comprising the conversation so far.
        /// </summary>
        public IReadOnlyCollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();

        /// <summary>
        /// Number between -2.0 and 2.0. Positive values penalize new tokens based on their existing frequency in the text so far, decreasing the
        /// model's likelihood to repeat the same line verbatim. Defaults to 0.
        /// </summary>
        [JsonPropertyName("frequency_penalty")]
        public double? FrequencyPenalty { get; set; }

        /// <summary>
        /// Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in the text so far, increasing the model's
        /// likelihood to talk about new topics. Defaults to 0.
        /// </summary>
        [JsonPropertyName("presence_penalty")]
        public double? PresencePenalty { get; set; }

        /// <summary>
        /// The maximum number of tokens that can be generated in the chat completion.
        /// </summary>
        [JsonPropertyName("max_tokens")]
        public int? MaxTokens { get; set; }

        /// <summary>
        /// How many chat completion choices to generate for each input message. Note that you will be charged based on the number of generated tokens
        /// across all of the choices. Keep n as 1 to minimize costs. Defaults to 1.
        /// </summary>
        public int? N { get; set; }

        /// <summary>
        /// This feature is in Beta. If specified, our system will make a best effort to sample deterministically, such that repeated requests with the
        /// same seed and parameters should return the same result. Determinism is not guaranteed, and you should refer to the system_fingerprint
        /// response parameter to monitor changes in the backend.
        /// </summary>
        public int? Seed { get; set; }

        /// <summary>
        /// What sampling temperature to use, between 0 and 2. Higher values like 0.8 will make the output more random, while lower values like 0.2 will make
        /// it more focused and deterministic. We generally recommend altering this or top_p but not both.
        /// </summary>
        public double? Temperature { get; set; }

        /// <summary>
        /// An alternative to sampling with temperature, called nucleus sampling, where the model considers the results of the tokens with top_p probability mass.
        /// So 0.1 means only the tokens comprising the top 10% probability mass are considered. We generally recommend altering this or temperature but not both.
        /// </summary>
        [JsonPropertyName("top_p")]
        public double? TopP { get; set; }

        /// <summary>
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// </summary>
        public string? User { get; set; }

        /// <summary>
        /// An object specifying the format that the model must output. Use <see cref="ResponseFormat.ResponseFormatFromType"/> to populate based on any type.
        /// </summary>
        [JsonPropertyName("response_format")]
        public ResponseFormat? ResponseFormat { get; set; }
    }
}
