using System.Configuration;

namespace Clocktower.Agent.Config
{
    [ConfigurationCollection(typeof(PlayerConfig))]
    internal class PlayerConfigCollection : ConfigurationElementCollection
    {
        public IEnumerable<PlayerConfig> PlayerConfigs
        {
            get
            {
                var enumerator = GetEnumerator();
                while (enumerator.MoveNext())
                {
                    yield return (PlayerConfig)enumerator.Current;
                }
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new PlayerConfig();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PlayerConfig)element).Name;
        }
    }
}
