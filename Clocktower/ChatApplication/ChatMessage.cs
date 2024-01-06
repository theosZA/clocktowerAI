using OpenAi;

namespace ChatApplication
{
    internal class ChatMessage
    {
        public Role Role { get; set; } = Role.User;
        public string Message { get; set; } = string.Empty;
    }
}
