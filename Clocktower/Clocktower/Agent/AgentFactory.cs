using Clocktower.Agent.Config;
using Clocktower.Game;
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

            var agentTasks = playerConfigs.Select(async config => await CreateAgent(config.AgentType, config.Model, config.Name, config.Personality, playerNames, setup.ScriptName, setup.Script, random));
            return await Task.WhenAll(agentTasks);
        }

        private static async Task<IAgent> CreateAgent(string agentType, string? model, string name, string personality, IReadOnlyCollection<string> playerNames, string scriptName, IReadOnlyCollection<Character> script, Random random)
        {
            return agentType switch
            {
                "Auto" => new HumanAgentForm(name, script, random) { AutoAct = true },
                "Human" => new HumanAgentForm(name, script, random),
                "Discord" => new DiscordAgent(await GetDiscordChatClient(), name, playerNames, scriptName, script),
                "Robot" => new RobotAgentForm(string.IsNullOrEmpty(model) ? DefaultModel : model, name, personality, playerNames, scriptName, script).Agent,
                _ => throw new ArgumentException($"Unknown agent type: {agentType}"),
            };
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

        private const string DefaultModel = "gpt-3.5-turbo";
    }
}
