using System.Configuration;

namespace Clocktower.Agent.Config
{
    internal class PlayerConfig : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name => this["name"] as string ?? string.Empty;

        [ConfigurationProperty("type", IsRequired = true)]
        public string AgentType => this["type"] as string ?? string.Empty;

    }
}
