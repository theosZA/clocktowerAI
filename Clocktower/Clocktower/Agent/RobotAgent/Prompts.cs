using Clocktower.Game;

namespace Clocktower.Agent.RobotAgent
{
    internal static class Prompts
    {
        public static string GetReasoningPrompt()
        {
            return "Use all the information you've learned so far together with logical reasoning to determine each player's likely alignment and character. " +
                   "Your answer should include a list, one item for each player with the following information: name; dead or alive; whether you believe they're good or evil " +
                   "(and in brackets how confident you are); character or characters that you think they're most likely to be (and in brackets how confident you are); " +
                   "and the main pieces of information that led you to this conclusion. For example:\r\n" +
                   "- **Zeke** - Alive - Good (very likely) - Slayer (almost certain) or Imp (unlikely) - I learned they were good on night 1 and so, " +
                   "unless I was drunk or poisoned, I trust the private claim that they made to me during day 1.";
        }
    }
}
