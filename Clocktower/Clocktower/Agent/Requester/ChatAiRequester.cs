using Clocktower.Agent.RobotAgent;
using Clocktower.Game;
using Clocktower.Options;
using System.Text;

namespace Clocktower.Agent.Requester
{
    internal class ChatAiRequester : IMarkupRequester
    {
        public ChatAiRequester(ClocktowerChatAi ai)
        {
            this.ai = ai;
        }

        public async Task<IOption> RequestCharacterForDemonKill(string prompt, IReadOnlyCollection<IOption> options)
        {
            var sb = new StringBuilder(prompt);
            sb.Append(" Please provide your reasoning, considering which characters would be most dangerous, or which players would be good to kill " +
                      "and what character you think they are. If you choose a character that isn't actually in play, the Storyteller will decide the kills tonight, " +
                      "and rarely will that be to your benefit. Conclude, not with the player, but with the specific character from the script you wish to kill.");
            return await ai.RequestCharacterSelection(options, sb.ToString());
        }

        public async Task<IOption> RequestPlayerForDemonKill(string prompt, IReadOnlyCollection<IOption> options)
        {
            var sb = new StringBuilder(prompt);

            var potentialKills = options.Select(option => ((PlayerOption)option).Player).ToList();
            sb.AppendFormattedText(" Please provide your reasoning, considering at least a few possible players to kill, as well as the " +
                                   "possibility of sinking a kill by targeting a dead player. Conclude with the name of the player to kill. The players you can target are: %P.",
                                   potentialKills);
            if (potentialKills.Any(player => !player.Alive))
            {
                AppendAliveSubsetOfPlayers(sb, potentialKills);
            }

            return await ai.RequestPlayerSelection(options, sb.ToString());
        }

        public async Task<IOption> RequestUseAbility(string prompt, IReadOnlyCollection<IOption> options)
        {
            var sb = new StringBuilder(prompt);
            sb.Append(" Respond with your reasoning.");
            return await ai.RequestUseAbility(options, sb.ToString());
        }

        public async Task<IOption> RequestCharacter(string prompt, IReadOnlyCollection<IOption> options)
        {
            var sb = new StringBuilder(prompt);
            if (options.Any(option => option is PassOption))
            {
                sb.Append(" Respond with your reasoning and then either provide a character from the script or don't provide a character if you want to save your ability for later.");
            }
            else
            {
                sb.Append(" Respond with your reasoning and a character from the script.");
            }
            return await ai.RequestCharacterSelection(options, sb.ToString());
        }

        public async Task<IOption> RequestPlayerTarget(string prompt, IReadOnlyCollection<IOption> options)
        {
            var sb = new StringBuilder(prompt);
            if (options.Any(option => option is PassOption))
            {
                sb.Append(" Respond with your reasoning and then either provide the name of the player you wish to target or don't provide the name of a player if you want to save your ability for later.");
            }
            else
            {
                sb.Append(" Respond with your reasoning and the name of the player you wish to target.");
            }
            return await ai.RequestPlayerSelection(options, sb.ToString());
        }

        public async Task<IOption> RequestTwoPlayersTarget(string prompt, IReadOnlyCollection<IOption> options)
        {
            var sb = new StringBuilder(prompt);
            if (options.Any(option => option is PassOption))
            {
                sb.Append(" Respond with your reasoning and then either provide the names of the two players you wish to target or don't provide any names if you want to save your ability for later.");
            }
            else
            {
                sb.Append(" Respond with your reasoning and the names of the two players you wish to target.");
            }
            return await ai.RequestTwoPlayersSelection(options, sb.ToString());
        }

        public async Task<IOption> RequestShenanigans(string prompt, IReadOnlyCollection<IOption> options)
        {
            var sb = new StringBuilder(prompt);

            sb.AppendLine(" You can explain your reasoning but should include one of the following options.");
            if (options.Any(option => option is JugglerOption))
            {
                sb.AppendFormattedText($"- `\"Claim\"=\"{TextUtilities.CharacterToText(Game.Character.Juggler)}\", \"Target\"=\"PLAYER_NAME AS CHARACTER, PLAYER_NAME AS CHARACTER, ...\"` with up to " +
                                             $"5 player-character pairs if you wish to claim %c and guess players as specific characters. (Players and characters may be repeated.)", Game.Character.Juggler);
                sb.AppendLine();
            }
            if (options.Any(option => option is SlayerShotOption))
            {
                sb.AppendFormattedText($"- `\"Claim\"=\"{TextUtilities.CharacterToText(Game.Character.Slayer)}\", \"Target\"=\"PLAYER_NAME\"` if you wish to claim %c and target the specified player.", Game.Character.Slayer);
                sb.AppendLine();
            }
            if (options.Any(option => option is PassOption))
            {
                sb.AppendLine("- `\"TakeAction\"=false` if you don't wish to use or bluff any of these abilities.");
            }
            if (options.Any(option => option is AlwaysPassOption))
            {
                sb.AppendLine("- `\"TakeAction\"=false, \"AlwaysPassInFuture\"=true` if you aren't bluffing any of these characters and so will skip this prompt in the future (unless you do have an ability you can use).");
            }

            return await ai.RequestShenanigans(options, sb.ToString());
        }

        public async Task<IOption> RequestNomination(string prompt, IReadOnlyCollection<IOption> options)
        {
            var potentialNominees = options.Where(option => option is PlayerOption)
                                           .Select(option => ((PlayerOption)option).Player)
                                           .ToList();

            var sb = new StringBuilder(prompt);
            sb.AppendLine(" Please provide your reasoning as an internal monologue, considering at least a few possible candidates for execution " +
                          "as well as the possibility of not nominating anyone. Normally you want to be nominating players you suspect are evil, especially the Demon. " +
                          "(If you're evil, then you want to be nominating players that you can frame as evil.) Though sometimes strategic considerations might make you " +
                          "want to nominate a good player instead. Also keep in mind how many votes will be required and whether you think that it's possible you'll get " +
                          "the needed votes for execution from your fellow players.");
            sb.AppendFormattedText("Then either provide the name of the player to nominate or don't provide the name of player if you don't wish to nominate at this time. " +
                                   "The players you can nominate are: %P.", potentialNominees);
            if (potentialNominees.Any(player => !player.Alive))
            {
                AppendAliveSubsetOfPlayers(sb, potentialNominees);
            }

            return await ai.RequestPlayerSelection(options, sb.ToString());
        }

        public async Task<IOption> RequestVote(string prompt, bool ghostVote, IReadOnlyCollection<IOption> options)
        {
            var nominee = ((VoteOption)options.First(option => option is VoteOption)).Nominee;

            var sb = new StringBuilder(prompt);
            sb.AppendLine();
            sb.AppendFormattedText("Please provide your reasoning as an internal monologue, considering the evidence surrounding their information and character, and pros and cons for executing %p." +
                                   " Some factors to keep in mind: If you're good, then executing other good players early on isn't a terrible idea as it can help some characters learn some information," +
                                   " but as you get closer to the end of the game you really need to be executing evil players, and especially in the final 3 or 4, you *must* be executing the demon." +
                                   " (If you're evil then you want to be executing valuable good players, and *not* the Demon, but at the same time you still need to look like your votes are helping the good players.)" +
                                   " It is not generally expected that you vote on yourself, and if there are already enough votes for the execution to pass, you may wish to hold off on over-voting " +
                                   " to give town the possibility for considering another nomination.");
            if (ghostVote)
            {
                sb.AppendLine();
                sb.Append("And since you only have one more vote available for the rest of the game, you need to decide whether or not this is the best time to spend that vote." +
                          " (Most commonly, ghost votes are kept until the last day when there are just 3 or 4 players left alive.)");
            }
            sb.AppendLine();
            sb.AppendFormattedText("Then decide whether you wish to execute %p or not.", nominee);

            return await ai.RequestVote(options, sb.ToString());
        }

        public async Task<IOption> RequestPlayerForChat(string prompt, IReadOnlyCollection<IOption> options)
        {
            var sb = new StringBuilder(prompt);
            var canPass = options.Any(option => option is PassOption);
            if (canPass)
            {
                sb.Append(" You may include some reasoning considering the merits of talking to particular players, and then either choose one player to talk to or leave the player choice blank to wait to see if someone else wants to talk to you. ");
            }
            else
            {
                sb.Append(" You may include some reasoning considering the merits of talking to particular players, but then you must choose one player to talk to. ");
            }
            var availablePlayers = options.Where(option => option is PlayerOption)
                                          .Select(option => ((PlayerOption)option).Player);
            sb.AppendFormattedText("The players you can talk to are: %P.", availablePlayers);
            return await ai.RequestPlayerSelection(options, sb.ToString());
        }

        public async Task<(string dialogue, bool endChat)> RequestMessageForChat(string prompt)
        {
            var sb = new StringBuilder(prompt);
            sb.Append(" You may include some reasoning for what you want to say. Only once you've heard what the other player has to say, and there's nothing more to discuss, you can choose to terminate the conversation.");
            return await ai.RequestChatDialogue(sb.ToString());
        }

        public async Task<string> RequestStatement(string prompt, IMarkupRequester.Statement statement)
        {
            var sb = new StringBuilder(prompt);

            switch (statement)
            {
                case IMarkupRequester.Statement.Morning:
                    sb.Append(" There's no need to waste time encouraging collaboration and transparency - instead this is a chance to share information (or misinformation) that you want to publicly share.");
                    break;

                case IMarkupRequester.Statement.Evening:
                    sb.Append(" This is a good chance to share speculation about what you think is happening in the game, who you think may be evil, and who would be a good player to execute today.");
                    break;
            }

            sb.Append(" (You may always choose to say nothing. You should provide your reasoning as an internal monologue for what you want to say, if anything.)");

            return await ai.RequestDialogue(sb.ToString());
        }

        public async Task RequestKazaliMinions(string prompt, KazaliMinionsOption kazaliMinionsOption)
        {
            var sb = new StringBuilder(prompt);
            sb.Append(" You should include your reasoning for who you will choose and which minion characters you'd prefer to have on your evil team.");
            await ai.RequestKazaliMinions(kazaliMinionsOption, sb.ToString());
        }

        private static void AppendAliveSubsetOfPlayers(StringBuilder stringBuilder, IEnumerable<Player> players)
        {
            var aliveSubset = players.Where(player => player.Alive).ToList();
            switch (aliveSubset.Count)
            {
                case 0:
                    stringBuilder.Append(" Of these players, none are still alive.");
                    break;

                case 1:
                    stringBuilder.AppendFormattedText(" Of these players, %P is still alive.", aliveSubset);
                    break;

                default:
                    stringBuilder.AppendFormattedText(" Of these players, %P are still alive.", aliveSubset);
                    break;
            }
        }

        private readonly ClocktowerChatAi ai;
    }
}
