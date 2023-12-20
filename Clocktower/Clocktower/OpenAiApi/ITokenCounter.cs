namespace Clocktower.OpenAiApi
{
    internal interface ITokenCounter
    {
        void NewTokenUsage(int promptTokens, int completionTokens, int totalTokens);
    }
}
