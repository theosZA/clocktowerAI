namespace Clocktower
{
    public enum Phase
    {
        Night,      // follow night order
        Morning,    // announce any deaths in the night
        Day,        // private chats
        Evening     // public discussion, nominations and execution
    }
}