using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace WDAdmin.WebUI.Infrastructure.CustomAttributes
{
    /// <summary>
    /// Extension to validation attributes allowing custom Regex parameters receiving input from Resource files
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class LocalizedValidationAttribute : ValidationAttribute, IClientValidatable
    {
        /// <summary>
        /// The _regex
        /// </summary>
        private Regex _regex;
        /// <summary>
        /// The _format
        /// </summary>
        private string _format;
        /// <summary>
        /// The _pattern
        /// </summary>
        private string _pattern;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedValidationAttribute"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public LocalizedValidationAttribute(string key)
        {
            _pattern = AppSettings.GetValidationPattern(key, CultureInfo.CurrentCulture);
            _regex = new Regex(_pattern);
            _format = AppSettings.GetValidationFormat(key, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Gets the client validation rules.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="context">The context.</param>
        /// <returns>IEnumerable&lt;ModelClientValidationRule&gt;.</returns>
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var name = metadata.GetDisplayName();
            return new[] { new ModelClientValidationRegexRule(FormatErrorMessage(name), _pattern), };
        }

        /// <summary>
        /// Determines whether the specified value of the object is valid.
        /// </summary>
        /// <param name="value">The value of the object to validate.</param>
        /// <returns>true if the specified value is valid; otherwise, false.</returns>
        public override bool IsValid(object value)
        {
            var str = Convert.ToString(value, CultureInfo.CurrentCulture);
            if (string.IsNullOrEmpty(str))
            {
                return true;
            }

            var match = _regex.Match(str);
            return ((match.Success && (match.Index == 0)) && (match.Length == str.Length));
        }

        /// <summary>
        /// Applies formatting to an error message, based on the data field where the error occurred.
        /// </summary>
        /// <param name="name">The name to include in the formatted message.</param>
        /// <returns>An instance of the formatted error message.</returns>
        public override string FormatErrorMessage(string name)
        {
            string errorMsg;
            if (string.IsNullOrEmpty(_format))
            {
                errorMsg = base.FormatErrorMessage(name);
            }
            else
            {
                errorMsg = string.Format(CultureInfo.CurrentCulture, ErrorMessageString, _format);
            }
            return errorMsg;
        }
    }
}