using Clocktower.Options;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Clocktower.Agent.Requester
{
    /// <summary>
    /// Responsible for requesting actions from agents by sending a prompt (in markup form) and getting a response in a format specific to that agent type.
    /// Prompts passed in to the Requester should be specific to the use-case, but not make any assumptions about the agent type. Implementations of
    /// <see cref="IMarkupRequester"/> can add their own markup text to the prompt if necessary to explain to the agent how they can provide a conforming response.
    /// </summary>
    internal interface IMarkupRequester
    {
        Task<IOption> RequestCharacterForDemonKill(string prompt, IReadOnlyCollection<IOption> options);

        Task<IOption> RequestPlayerForDemonKill(string prompt, IReadOnlyCollection<IOption> options);

        Task<IOption> RequestUseAbility(string prompt, IReadOnlyCollection<IOption> options);

        Task<IOption> RequestCharacter(string prompt, IReadOnlyCollection<IOption> options);

        Task<IOption> RequestPlayerTarget(string prompt, IReadOnlyCollection<IOption> options);

        Task<IOption> RequestTwoPlayersTarget(string prompt, IReadOnlyCollection<IOption> options);

        Task<IOption> RequestShenanigans(string prompt, IReadOnlyCollection<IOption> options);

        Task<IOption> RequestNomination(string prompt, IReadOnlyCollection<IOption> options);

        Task<IOption> RequestVote(string prompt, bool ghostVote, IReadOnlyCollection<IOption> options);

        Task<IOption> RequestPlayerForChat(string prompt, IReadOnlyCollection<IOption> options);

        Task<(string dialogue, bool endChat)> RequestMessageForChat(string prompt);

        enum Statement
        {
            Morning,
            Evening,
            RollCall,
            SelfNomination,
            Prosection,
            Defence
        };

        Task<string> RequestStatement(string prompt, Statement statement);

    }
}
