using WDAdmin.Domain.Entities;

namespace WDAdmin.WebUI.Infrastructure
{
    /// <summary>
    /// Helper class getting the resource values from resource file
    /// </summary>
    public interface ILogListener
    {
        /// <summary>
        /// Logs the specified title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="logType">Type of the log.</param>
        /// <param name="entryType">Type of the entry.</param>
        void Log(string title, LogType logType, LogEntryType entryType);
        /// <summary>
        /// Logs the specified title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="logType">Type of the log.</param>
        /// <param name="entryType">Type of the entry.</param>
        void Log(string title, string message, LogType logType, LogEntryType entryType);
        /// <summary>
        /// Logs the specified title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="otherInfo">The other information.</param>
        /// <param name="logType">Type of the log.</param>
        /// <param name="entryType">Type of the entry.</param>
        void Log(string title, string message, string otherInfo, LogType logType, LogEntryType entryType);
    }
}
