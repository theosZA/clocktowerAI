using System.Text.Json.Serialization;

namespace OpenAi.ChatCompletionApi.Model
{
    internal class ChatCompletionResponse
    {
        /// <summary>
        /// A unique identifier for the chat completion.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// A list of chat completion choices. Can be more than one if n is greater than 1.
        /// </summary>
        public IReadOnlyCollection<ChatChoice> Choices { get; set; } = new List<ChatChoice>();

        /// <summary>
        /// The Unix timestamp (in seconds) of when the chat completion was created.
        /// </summary>
        public int Created { get; set; } = 0;

        /// <summary>
        /// The model used for the chat completion.
        /// </summary>
        public string Model { get; set; } = string.Empty;

        /// <summary>
        /// This fingerprint represents the backend configuration that the model runs with. Can be used in conjunction with
        /// the seed request parameter to understand when backend changes have been made that might impact determinism.
        /// </summary>
        [JsonPropertyName("system_fingerprint")]
        public string SystemFingerprint { get; set; } = string.Empty;

        /// <summary>
        /// The object type, which is always chat.completion.
        /// </summary>
        public string Object { get; set; } = "chat.completion";

        /// <summary>
        /// Usage statistics for the completion request.
        /// </summary>
        public Usage Usage { get; set; } = new Usage();
    }
}
