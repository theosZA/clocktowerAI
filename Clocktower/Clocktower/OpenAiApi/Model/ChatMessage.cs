namespace Clocktower.OpenAiApi.Model
{
    public class ChatMessage
    {
        public string Role { get; set; } = "user";

        public string Content { get; set; } = string.Empty;

        public string? Name { get; set; }
    }
}
