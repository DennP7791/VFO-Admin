using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WDAdmin.Domain.Entities;

namespace WDAdmin.WebUI.Infrastructure
{
    /// <summary>
    /// Service class for log listeners
    /// </summary>
    public static class Logger //http://support.microsoft.com/default.aspx?kbid=323076
    {
        /// <summary>
        /// List of log listeners
        /// </summary>
        private static List<ILogListener> logListeners = new List<ILogListener>();

        /// <summary>
        /// Logs the specified title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="logType">Type of the log.</param>
        /// <param name="entryType">Type of the entry.</param>
        public static void Log(string title, LogType logType, LogEntryType entryType)
        {
            foreach (var listerner in logListeners)
            {
                listerner.Log(title, logType, entryType);
            }
        }

        /// <summary>
        /// Logs the specified title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="logType">Type of the log.</param>
        /// <param name="entryType">Type of the entry.</param>
        public static void Log(string title, string message, LogType logType, LogEntryType entryType)
        {
            foreach (var listerner in logListeners)
            {
                listerner.Log(title, message, logType, entryType);
            }
        }

        /// <summary>
        /// Logs the specified title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="otherInfo">The other information.</param>
        /// <param name="logType">Type of the log.</param>
        /// <param name="entryType">Type of the entry.</param>
        public static void Log(string title, string message, string otherInfo, LogType logType, LogEntryType entryType)
        {
            foreach (var listerner in logListeners)
            {
                listerner.Log(title, message, otherInfo, logType, entryType);
            }
        }

        /// <summary>
        /// Add log listener
        /// </summary>
        /// <param name="listener">ILogListener</param>
        public static void AddLogListener(ILogListener listener)
        {
            logListeners.Add(listener);
        }

        /// <summary>
        /// Remove log listener
        /// </summary>
        /// <param name="listener">ILogListener</param>
        public static void RemoveLogListener(ILogListener listener)
        {
            if (logListeners.Contains(listener))
            {
                logListeners.Remove(listener);
            }
        }
    }
}
