using System.Configuration;

namespace ChatApplication.Config
{
    [ConfigurationCollection(typeof(ModelConfig))]
    internal class ModelConfigCollection : ConfigurationElementCollection
    {
        public IEnumerable<ModelConfig> ModelConfigs
        {
            get
            {
                var enumerator = GetEnumerator();
                while (enumerator.MoveNext())
                {
                    yield return (ModelConfig)enumerator.Current;
                }
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ModelConfig();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ModelConfig)element).Name;
        }
    }
}
