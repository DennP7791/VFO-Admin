using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace WDAdmin.WebUI.Infrastructure.Various
{
    /// <summary>
    /// Helper class to resolve culture related issues
    /// </summary>
    public static class CultureHelper
    {
        /// <summary>
        /// Change culture of the current thread
        /// Used when Application_AcquireRequestState does give the correct results
        /// </summary>
        /// <param name="culture">Culture value</param>
        public static void ChangeThreadCulture(string culture)
        {
            var ci = new CultureInfo(culture);
            System.Threading.Thread.CurrentThread.CurrentUICulture = ci;
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(ci.Name);
        }

        /// <summary>
        /// Check if the given culture value is valid
        /// </summary>
        /// <param name="cultureName">Culture value</param>
        /// <returns>Result of the evaluation (true/false)</returns>
        public static bool IsValidCultureName(string cultureName)
        {
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            return cultures.Any(culture => culture.Name == cultureName);
        }
    }
}