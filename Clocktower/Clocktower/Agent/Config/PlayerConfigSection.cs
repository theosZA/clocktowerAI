using System.Configuration;

namespace Clocktower.Agent.Config
{
    internal class PlayerConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public PlayerConfigCollection Players
        {
            get { return (PlayerConfigCollection)this[""]; }
        }
    }
}
