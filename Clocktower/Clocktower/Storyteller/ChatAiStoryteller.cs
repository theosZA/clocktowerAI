using Clocktower.Agent;
using Clocktower.Agent.RobotAgent.Model;
using Clocktower.Game;
using Clocktower.Options;
using OpenAi;

namespace Clocktower.Storyteller
{
    internal class ChatAiStoryteller
    {
        public ChatAiStoryteller(OpenAiChat chat, string chatModel, IReadOnlyCollection<string> playerNames, string scriptName, IReadOnlyCollection<Character> script)
        {
            this.chat = chat;
            this.chatModel = chatModel;

            this.playerNames = playerNames;
            this.scriptName = scriptName;
            this.script = script;

            chat.SystemMessage = GetSystemMessage();
        }

        public async Task<IOption> Prompt(string prompt, IReadOnlyCollection<IOption> options)
        {
            if (options.All(option => option is PlayerOption || option is PassOption))
            {
                return await RequestOptionFromJson<PlayerSelection>(options, prompt);
            }

            if (options.All(option => option is CharacterOption || option is PassOption))
            {
                return await RequestOptionFromJson<CharacterSelection>(options, prompt);
            }

            if (options.All(option => option is DirectionOption))
            {
                return await RequestOptionFromJson<DirectionSelection>(options, prompt);
            }

            if (options.All(option => option is CharacterForTwoPlayersOption || option is NoOutsiders))
            {
                return await RequestOptionFromJson<CharacterForTwoPlayersSelection>(options, prompt);
            }

            if (options.All(option => option is NumberOption))
            {
                return await RequestOptionFromJson<NumberSelection>(options, prompt);
            }

            if (options.All(option => option is PlayerListOption || option is ThreePlayersOption || option is TwoPlayersOption))
            {
                return await RequestOptionFromJson<PlayersSelection>(options, prompt);
            }

            if (options.All(option => option is ThreeCharactersOption))
            {
                return await RequestOptionFromJson<CharactersSelection>(options, prompt);
            }

            if (options.Any(option => option is YesOption) && options.Any(option => option is NoOption || option is PassOption))
            {
                return await RequestOptionFromJson<YesNoSelection>(options, prompt);
            }

            throw new InvalidOperationException("Unsupported combination of options provided for AI storyteller");
        }

        public async Task<string> PromptForText(string prompt)
        {
            return (await RequestObject<TextResponse>(prompt))?.Response ?? string.Empty;
        }

        private async Task<IOption> RequestOptionFromJson<T>(IReadOnlyCollection<IOption> options, string prompt) where T : IOptionSelection
        {
            for (int retry = 0; retry < 3; retry++)
            {
                var response = await RequestObject<T>(prompt);
                var result = response.PickOption(options);
                if (result != null)
                {
                    return result;
                }
                prompt = response.NoMatchingOptionPrompt(options);
            }
            return options.First(option => option is PassOption);
        }

        private async Task<T> RequestObject<T>(string prompt)
        {
            if (!string.IsNullOrEmpty(prompt))
            {
                chat.AddUserMessage(prompt);
            }
            return await chat.GetAssistantResponse<T>(chatModel) ?? throw new InvalidDataException($"AI Storyteller did not respond with a valid {typeof(T).Name} object");
        }

        private string GetSystemMessage()
        {
            return $"You are the *Storyteller*, the individual running the game 'Blood on the Clocktower'.\r\n" +
            "'Blood on the Clocktower' is a social deduction game where the players are divided into good Townsfolk and Outsiders and evil Minions and Demons. " +
            "The evil players know who is on which side and they win when there are only two players left alive, one of which is their Demon. " +
            "The good players don't know who is on which side and must use their character abilities and social skills to determine who the evil players are. The good players win if the Demon dies, usually by executing them. " +
            "The game is split into two phases: a day phase and a night phase.\r\n" +
            "During the day, the players talk among themselves. Each player will have a secret identity, a unique character from the script. Generally, the good players share whatever they know and attempt to find out who is who. " +
            "Most good players will be telling the truth, but some have an incentive to lie. And evil players should definitely be lying (except in private conversations with their fellow evil players)! " +
            "At the end of each day there will be a chance to nominate players for execution. Only one player may be executed each day, and the good team usually wants to execute a player each day since executing the Demon is how they win. " +
            "Each player only gets one nomination each day, and the same player can't be nominated more than once in a day. Whoever has the most votes is executed. " +
            "This player needs a vote tally of at least 50% of the living players or no execution occurs. On a tie, neither player is executed.\r\n" +
            "During the night, some players will get a chance to secretly use their ability or gain some type of information. This includes the Demon who uses their ability to kill off the players. " +
            "Most players will die - but death is not the end. Some players may even want to die, as they gain information when they do. If a player is dead, they may still participate in the game, may still talk, " +
            "and still wins or loses with their team. In fact, the game is usually decided by the votes and opinions of the dead players. " +
            "When a player dies, they lose their character ability, they may no longer nominate, and they have only one vote for the rest of the game.\r\n" +
            "There is a lot of information in this game. However, some of it might be wrong. If a player is drunk or poisoned, they have no ability, but the Storyteller should pretend that they do: " +
            "their ability won't work and any information they get from they ability should usually be incorrect.\r\n" +
            "As the Storyteller, it is your duty to run a fun and interesting game for all the players. Part of this is balancing the game between the good and evil teams, generally by making sure the evil " +
            "team isn't too easy to pinpoint with good abilities and backing up the evil team's bluffs when you have the opportunity to feed false information to drunk or poisoned players.\r\n" +
            TextBuilder.ScriptToText(scriptName, script) +
            TextBuilder.SetupToText(playerNames.Count, script) +
            TextBuilder.PlayersToText(playerNames);
        }

        private readonly OpenAiChat chat;
        private readonly string chatModel;

        private readonly IReadOnlyCollection<string> playerNames;
        private readonly string scriptName;
        private readonly IReadOnlyCollection<Character> script;
    }
}
