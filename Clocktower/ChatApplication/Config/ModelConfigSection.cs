using System.Configuration;

namespace ChatApplication.Config
{
    internal class ModelConfigSection : ConfigurationSection
    {
        public static ModelConfigSection? Instance => ConfigurationManager.GetSection("ModelConfig") as ModelConfigSection;

        public static IEnumerable<string> ModelDescriptions => Instance?.Models?.ModelConfigs?.Select(model => model.Description) ?? Enumerable.Empty<string>();

        public static string? GetModelName(string modelDescription) => Instance?.Models?.ModelConfigs?.FirstOrDefault(model => model.Description == modelDescription)?.Name;

        [ConfigurationProperty("", IsDefaultCollection = true)]
        public ModelConfigCollection Models
        {
            get { return (ModelConfigCollection)this[""]; }
        }
    }
}
