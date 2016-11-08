using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;

namespace WDAdmin.WebUI
{
    /// <summary>
    /// Class AppSettings.
    /// </summary>
    public static class AppSettings
    {
        /// <summary>
        /// Gets the address from.
        /// </summary>
        /// <value>The address from.</value>
        public static string AddressFrom
        {
            get
            {
                string s = "S";
                return Setting<string>("AddressFrom");
            }
        }
        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public static string DisplayName
        {
            get {
                return Setting<string>("DisplayName");
            }
        }
        /// <summary>
        /// Gets the default culture.
        /// </summary>
        /// <value>The default culture.</value>
        public static CultureInfo DefaultCulture
        {
            get {
                return new CultureInfo(Setting<string>("DefaultCulture"));
            }
        }

        /// <summary>
        /// Gets the validation pattern.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>System.String.</returns>
        public static string GetValidationPattern(string key, CultureInfo culture)
        {
            return Setting<string>(string.Format("ValidationPattern{0}_{1}", key, culture));
        }

        /// <summary>
        /// Gets the validation format.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>System.String.</returns>
        public static string GetValidationFormat(string key, CultureInfo culture)
        {
            return Setting<string>(string.Format("ValidationFormat{0}_{1}", key, culture));
        }

        /// <summary>
        /// Settings the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns>T.</returns>
        /// <exception cref="System.Exception"></exception>
        private static T Setting<T>(string name)
        {
            string value = ConfigurationManager.AppSettings[name];

            if (value == null)
            {
                throw new Exception(String.Format("Could not find setting '{0}',", name));
            }

            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }
    }
}