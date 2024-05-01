using Clocktower.Game;
using System.Configuration;

namespace Clocktower.Agent.Config
{
    internal class PlayerConfig : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name => this["name"] as string ?? string.Empty;

        [ConfigurationProperty("type", IsRequired = true)]
        public string AgentType => this["type"] as string ?? string.Empty;

        [ConfigurationProperty("model", IsRequired = false)]
        public string Model => this["model"] as string ?? string.Empty;

        [ConfigurationProperty("personality", IsRequired = false)]
        public string Personality => this["personality"] as string ?? string.Empty;

        [ConfigurationProperty("alignment", IsRequired = false)]
        public Alignment? Alignment => this["alignment"] as Alignment?;

        [ConfigurationProperty("character", IsRequired = false)]
        public Character? Character => this["character"] as Character?;
    }
}
