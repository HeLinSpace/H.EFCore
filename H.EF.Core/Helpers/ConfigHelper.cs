using Microsoft.Extensions.Configuration;

namespace H.EF.Core.Helpers
{
    public class ConfigHelper
    {
        private static readonly IConfiguration configuration;
        static ConfigHelper()
        {
            configuration = IoCContainer.Resolve<IConfiguration>();
        }
        public static IConfigurationSection GetSection(string key)
        {
            return configuration.GetSection(key);
        }

        public static string GetConfigurationValue(string section, string key)
        {
            return GetSection(section)?[key];
        }

        public static string GetConnectionString(string key)
        {
            return configuration.GetConnectionString(key);
        }
    }
}
