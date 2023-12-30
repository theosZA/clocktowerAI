namespace OpenAi
{
    public interface ITokenCounter
    {
        void NewTokenUsage(int promptTokens, int completionTokens, int totalTokens);
    }
}
