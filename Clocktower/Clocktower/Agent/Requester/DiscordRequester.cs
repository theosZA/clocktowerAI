using Clocktower.Game;
using Clocktower.Options;
using System.Text;

namespace Clocktower.Agent.Requester
{
    internal class DiscordRequester : IMarkupRequester
    {
        public DiscordRequester(TextPlayerPrompter prompter)
        {
            this.prompter = prompter;
        }

        public async Task<IOption> RequestCharacterForDemonKill(string prompt, IReadOnlyCollection<IOption> options)
        {
            return await RequestOption(prompt, options);
        }

        public async Task<IOption> RequestPlayerForDemonKill(string prompt, IReadOnlyCollection<IOption> options)
        {
            return await RequestOption(prompt, options);
        }

        public async Task<IOption> RequestUseAbility(string prompt, IReadOnlyCollection<IOption> options)
        {
            var sb = new StringBuilder(prompt);
            sb.AppendLine(" Respond with `YES` or `NO`.");

            return await RequestOption(sb.ToString(), options);
        }

        public async Task<IOption> RequestCharacter(string prompt, IReadOnlyCollection<IOption> options)
        {
            return await RequestOption(prompt, options);
        }

        public async Task<IOption> RequestPlayerTarget(string prompt, IReadOnlyCollection<IOption> options)
        {
            return await RequestOption(prompt, options);
        }

        public async Task<IOption> RequestTwoPlayersTarget(string prompt, IReadOnlyCollection<IOption> options)
        {
            var sb = new StringBuilder(prompt);
            sb.AppendLine(" Respond with the names of two players formatted like: `player_1 AND player_2`.");

            return await RequestOption(sb.ToString(), options);
        }

        public async Task<IOption> RequestShenanigans(string prompt, IReadOnlyCollection<IOption> options)
        {
            var sb = new StringBuilder(prompt);

            sb.AppendLine();
            if (options.Any(option => option is SlayerShotOption))
            {
                sb.AppendFormattedText("- `Slayer: player_name` if you wish to claim %c and target the specified player.", Character.Slayer);
                sb.AppendLine();
            }
            if (options.Any(option => option is JugglerOption))
            {
                sb.AppendFormattedText("- `Juggler: player_name as character, player_name as character, ...` with up to 5 player-character pairs if you wish to claim %c and guess players as specific characters. (Players and characters may be repeated.)",
                                       Character.Juggler);
                sb.AppendLine();
            }
            if (options.Any(option => option is PassOption))
            {
                sb.AppendLine("- `PASS` if you don't wish to use or bluff any of these abilities.");
            }
            if (options.Any(option => option is AlwaysPassOption))
            {
                sb.AppendLine("- `ALWAYS PASS` if you never wish to bluff any of these abilities (though you'll still be prompted if you do have an ability you can use).");
            }

            return await prompter.RequestShenanigans(options, sb.ToString());
        }

        public async Task<string> RequestStatement(string prompt, IMarkupRequester.Statement statement)
        {
            var sb = new StringBuilder(prompt);
            sb.AppendLine(" You may respond with `PASS` if you don't want to say anything.");

            return await prompter.RequestDialogue(sb.ToString());
        }

        public async Task<IOption> RequestNomination(string prompt, IReadOnlyCollection<IOption> options)
        {
            var sb = new StringBuilder(prompt);
            sb.AppendLine(" Respond with the name of the player to nominate, or `PASS` if you don't wish to nominate anyone.");

            return await RequestOption(sb.ToString(), options);
        }

        public async Task<IOption> RequestVote(string prompt, bool ghostVote, IReadOnlyCollection<IOption> options)
        {
            var sb = new StringBuilder(prompt);
            sb.AppendLine(" Respond with `EXECUTE` or `PASS`.");

            return await RequestOption(sb.ToString(), options);
        }

        public async Task<IOption> RequestPlayerForChat(string prompt, IReadOnlyCollection<IOption> options)
        {
            var sb = new StringBuilder(prompt);

            if (options.Any(option => option is PassOption))
            {
                sb.AppendLine(" Respond with the name of the player to chat to, or `PASS` if you want to wait to see who wants to talk to you.");
            }
            else
            {
                sb.AppendLine(" Respond with the name of the player to chat to.");
            }

            return await RequestOption(sb.ToString(), options);
        }

        public async Task<(string dialogue, bool endChat)> RequestMessageForChat(string prompt)
        {
            var sb = new StringBuilder(prompt);
            sb.Append(" To end the conversation respond with `PASS` or conclude what you wish to say with \"Goodbye\".");

            return await prompter.RequestChatDialogue(sb.ToString());
        }

        private async Task<IOption> RequestOption(string prompt, IReadOnlyCollection<IOption> options)
        {
            return await prompter.RequestChoice(options, prompt);
        }

        private readonly TextPlayerPrompter prompter;
    }
}
