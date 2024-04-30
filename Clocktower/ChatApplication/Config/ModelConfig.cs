using System.Configuration;

namespace ChatApplication.Config
{
    internal class ModelConfig : ConfigurationElement
    {
        [ConfigurationProperty("description", IsRequired = true)]
        public string Description => this["description"] as string ?? string.Empty;

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name => this["name"] as string ?? string.Empty;
    }
}
