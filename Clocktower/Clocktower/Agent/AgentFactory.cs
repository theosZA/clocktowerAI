using Clocktower.Agent.Config;
using Clocktower.Agent.Notifier;
using Clocktower.Agent.Observer;
using Clocktower.Agent.Requester;
using Clocktower.Agent.RobotAgent;
using Clocktower.Game;
using Clocktower.Setup;
using DiscordChatBot;
using System.Configuration;

namespace Clocktower.Agent
{
    internal static class AgentFactory
    {
        public static async Task<IEnumerable<IAgent>> CreateAgentsFromConfig(IGameSetup setup, Random random)
        {
            var playerConfigsSection = ConfigurationManager.GetSection("PlayerConfig") as PlayerConfigSection ?? throw new Exception("Invalid or missing PlayerConfig section");
            var playerConfigs = playerConfigsSection.Players.PlayerConfigs.Take(setup.PlayerCount).ToList();
            var playerNames = playerConfigs.Select(config => config.Name).ToList();

            var agentTasks = playerConfigs.Select(async config => await CreateAgent(config.AgentType, config.Model, config.ReasoningModel, config.Name, config.Personality, playerNames, setup.ScriptName, setup.Script, random));
            return await Task.WhenAll(agentTasks);
        }

        private static async Task<IAgent> CreateAgent(string agentType, string? chatModel, string? reasoningModel, string name, string personality, IReadOnlyCollection<string> playerNames, string scriptName, IReadOnlyCollection<Character> script, Random random)
        {
            return agentType switch
            {
                "Auto" => CreateLocalHumanAgent(name, playerNames, scriptName, script, random, autoAct: true),
                "Human" => CreateLocalHumanAgent(name, playerNames, scriptName, script, random),
                "Discord" => await CreateDiscordHumanAgent(name, playerNames, scriptName, script),
                "Robot" => CreateRobotAgent(string.IsNullOrEmpty(chatModel) ? DefaultChatModel : chatModel,
                                            string.IsNullOrEmpty(reasoningModel) ? DefaultReasoningModel : reasoningModel,
                                            name, personality, playerNames, scriptName, script),
                _ => throw new ArgumentException($"Unknown agent type: {agentType}"),
            };
        }

        private static IAgent CreateRobotAgent(string chatModel, string reasoningModel, string name, string personality, IReadOnlyCollection<string> playerNames, string scriptName, IReadOnlyCollection<Character> script)
        {
            var chatAi = new ClocktowerChatAi(chatModel, reasoningModel, name, personality, playerNames, scriptName, script);
            var chatAiNotifier = new ChatAiNotifier(chatAi);
            var chatAiRequester = new ChatAiRequester(chatAi);
            var observer = new TextObserver(chatAiNotifier);
            var agent = new TextAgent(name, playerNames, scriptName, script, observer, chatAiNotifier, chatAiRequester);

            var robotTriggers = new RobotTriggers(name, chatAi);
            observer.OnDay += robotTriggers.OnDay;
            observer.OnNight += robotTriggers.OnNight;
            observer.OnNominationsStart += robotTriggers.OnNominationStart;
            agent.OnAssignCharacter += robotTriggers.OnAssignCharacter;
            agent.OnChangeAlignment += robotTriggers.OnChangeAlignment;
            agent.OnGainingCharacterAbility += robotTriggers.OnGainingCharacterAbility;
            agent.OnDead += robotTriggers.OnDead;
            agent.YourDemonIs += robotTriggers.YourDemonIs;
            agent.YourMinionsAre += robotTriggers.YourMinionsAre;
            agent.OnEndGame += robotTriggers.EndGame;

            var form = new RobotAgentForm(robotTriggers);
            chatAi.OnChatMessage += form.OnChatMessage;
            chatAi.OnDaySummary += form.OnDaySummary;
            chatAi.OnTokenCount += form.OnTokenCount;
            agent.OnStartGame += () =>
            {
                form.Show();
                return Task.CompletedTask;
            };

            return agent;
        }

        private static IAgent CreateLocalHumanAgent(string name, IReadOnlyCollection<string> playerNames, string scriptName, IReadOnlyCollection<Character> script, Random random, bool autoAct = false)
        {
            var form = new HumanAgentForm(name, script, random)
            {
                AutoAct = autoAct
            };

            var notifier = new RichTextBoxNotifier(form.Output);
            var observer = new TextObserver(notifier);
            var requester = new LocalHumanRequester(form, notifier, random);
            observer.OnPrivateChatStart += requester.OnPrivateChatStart;

            var agent = new TextAgent(name, playerNames, scriptName, script, observer, notifier, requester);
            agent.OnStartGame += () =>
            {
                form.Show();
                return Task.CompletedTask;
            };
            agent.OnAssignCharacter += form.AssignCharacter;
            agent.OnChangeAlignment += form.ChangeAlignment;
            agent.OnGainingCharacterAbility += form.OnGainCharacterAbility;
            agent.OnDead += form.YouAreDead;

            return agent;
        }

        private static async Task<IAgent> CreateDiscordHumanAgent(string name, IReadOnlyCollection<string> playerNames, string scriptName, IReadOnlyCollection<Character> script)
        {
            var chatClient = await GetDiscordChatClient();
            var notifier = new DiscordNotifier(chatClient);
            var observer = new TextObserver(notifier);

            var prompter = new TextPlayerPrompter(name);
            prompter.SendMessageAndGetResponse += notifier.SendMessageAndGetResponse;
            var requester = new DiscordRequester(prompter);

            return new TextAgent(name, playerNames, scriptName, script, observer, notifier, requester);
        }

        private static async Task<ChatClient> GetDiscordChatClient()
        {
            if (discordChatClient == null)
            {
                discordChatClient = new ChatClient();
                await discordChatClient.Start();
            }
            return discordChatClient;
        }

        private static ChatClient? discordChatClient;

        private const string DefaultChatModel = "gpt-4o-mini";
        private const string DefaultReasoningModel = "o1-mini";
    }
}
