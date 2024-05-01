using Clocktower.Agent.Config;
using Clocktower.Game;
using System.Configuration;

namespace Clocktower.Agent
{
    internal static class AgentFactory
    {
        public static IEnumerable<IAgent> CreateAgentsFromConfig(IGameSetup setup, Random random)
        {
            var playerConfigsSection = ConfigurationManager.GetSection("PlayerConfig") as PlayerConfigSection ?? throw new Exception("Invalid or missing PlayerConfig section");
            var playerConfigs = playerConfigsSection.Players.PlayerConfigs.Take(setup.PlayerCount).ToList();
            var playerNames = playerConfigs.Select(config => config.Name).ToList();
            return playerConfigs.Select(config => CreateAgent(config.AgentType, config.Model, config.Name, config.Personality, playerNames, setup.Script, random));
        }

        private static IAgent CreateAgent(string agentType, string? model, string name, string personality, IReadOnlyCollection<string> playerNames, IReadOnlyCollection<Character> script, Random random)
        {
            return agentType switch
            {
                "Auto" => new HumanAgentForm(name, script, random) { AutoAct = true },
                "Human" => new HumanAgentForm(name, script, random),
                "Robot" => new RobotAgentForm(string.IsNullOrEmpty(model) ? DefaultModel : model, name, personality, playerNames, script).Agent,
                _ => throw new ArgumentException($"Unknown agent type: {agentType}"),
            };
        }

        private const string DefaultModel = "gpt-3.5-turbo";
    }
}
