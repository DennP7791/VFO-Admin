using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using WDAdmin.Resources;

namespace WDAdmin.WebUI.Infrastructure
{
    /// <summary>
    /// Handler for exchanging DB names into .resx file names
    /// </summary>
    public sealed class ResourceHandler
    {
        /// <summary>
        /// The _instance
        /// </summary>
        private static ResourceHandler _instance;

        /// <summary>
        /// GetInstance of the singleton
        /// </summary>
        /// <value>The get instance.</value>
        public static ResourceHandler GetInstance
        {
            get { return _instance ?? (_instance = new ResourceHandler()); }
        }

        /// <summary>
        /// Get single resource value - culture specified
        /// </summary>
        /// <param name="key">Key value in LangResources file</param>
        /// <param name="culture">Culture value</param>
        /// <returns>Localized string</returns>
        public string GetResource(string key, string culture)
        {
            var manager = new ResourceManager("WDAdmin.Resources.LangResources", typeof(LangResources).Assembly);
            var cultInfo = new CultureInfo(culture);
            return manager.GetString(key, cultInfo);
        }

        /// <summary>
        /// Generate Dictionary with resource values - culture specified
        /// </summary>
        /// <param name="keys">List of keys to strings to be localized</param>
        /// <param name="culture">Culture value</param>
        /// <returns>Dictionary with keys and localized strings</returns>
        public Dictionary<string, string> GetResources(List<string> keys, string culture)
        {
            var manager = new ResourceManager("WDAdmin.Resources.LangResources", typeof(LangResources).Assembly);
            var cultInfo = new CultureInfo(culture);
            return keys.ToDictionary(key => key, key => manager.GetString(key,cultInfo));
        }
    }
}