using System;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Globalization;

namespace WDAdmin.Domain
{
    /// <summary>
    /// Extension to validation attributes allowing custom regex parameters receiving input from Resource files
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class CustomRegexAttribute : ValidationAttribute
    {
        /// <summary>
        /// The _reg
        /// </summary>
        private Regex _reg;
        /// <summary>
        /// Gets or sets the pattern.
        /// </summary>
        /// <value>The pattern.</value>
        private string Pattern { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomRegexAttribute"/> class.
        /// </summary>
        /// <param name="resourceType">Type of the resource.</param>
        /// <param name="resourceName">Name of the resource.</param>
        public CustomRegexAttribute(Type resourceType, string resourceName)
        {
            Pattern = ResourceHelper.GetResourceLookup(resourceType, resourceName);
        }

        /// <summary>
        /// Determines whether the specified value of the object is valid.
        /// </summary>
        /// <param name="value">The value of the object to validate.</param>
        /// <returns>true if the specified value is valid; otherwise, false.</returns>
        public override bool IsValid(object value)
        {
            string str = Convert.ToString(value, CultureInfo.CurrentCulture);
            if (string.IsNullOrEmpty(str))
            {
                return true;
            }

            _reg = new Regex(Pattern);
            Match match = _reg.Match(str);
            return ((match.Success && (match.Index == 0)) && (match.Length == str.Length)); 
        }
    }

    /// <summary>
    /// Helper class getting the resource values from resource file
    /// </summary>
    public class ResourceHelper
    {
        public static string GetResourceLookup(Type resourceType, string resourceName)
        {
            if ((resourceType != null) && (resourceName != null))
            {
                var property = resourceType.GetProperty(resourceName, BindingFlags.Public | BindingFlags.Static);
                
                if (property == null)
                {
                    throw new InvalidOperationException(string.Format("Resource Type Does Not Have Property"));
                }
                
                if (property.PropertyType != typeof(string))
                {
                    throw new InvalidOperationException(string.Format("Resource Property is Not String Type"));
                }

                return (string)property.GetValue(null, null);
            }
            
            return null;
        }
    } 
}
