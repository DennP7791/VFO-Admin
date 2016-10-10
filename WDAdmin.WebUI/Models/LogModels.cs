using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WDAdmin.Resources;
using WDAdmin.Domain.Entities;

namespace WDAdmin.WebUI.Models
{
    /// <summary>
    /// Model for LogIndex page
    /// </summary>
    public class LogViewModel
    {
        /// <summary>
        /// Gets or sets the log entries.
        /// </summary>
        /// <value>The log entries.</value>
        public List<Log> LogEntries { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [no log entries].
        /// </summary>
        /// <value><c>true</c> if [no log entries]; otherwise, <c>false</c>.</value>
        public bool NoLogEntries { get; set; }
    }

    /// <summary>
    /// Model for LogDetails page
    /// </summary>
    public class LogDetailsModel
    {
        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>The time.</value>
        [Display(ResourceType = typeof(LangResources), Name = "Timestamp")]
        public string Time { get; set; }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        [Display(ResourceType = typeof(LangResources), Name = "Message")]
        public string Message { get; set; }
        /// <summary>
        /// Gets or sets the information.
        /// </summary>
        /// <value>The information.</value>
        [Display(ResourceType = typeof(LangResources), Name = "Information")]
        public string Information { get; set; }
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [Display(ResourceType = typeof(LangResources), Name = "Title")]
        public string Title { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [Display(ResourceType = typeof(LangResources), Name = "Type")]
        public string Type { get; set; }
    }
}