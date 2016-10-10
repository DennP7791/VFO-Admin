using System;
using System.Transactions;
using System.Web;
using WDAdmin.Domain.Entities;
using WDAdmin.Domain.Abstract;
using log4net;
using System.Reflection;

namespace WDAdmin.WebUI.Infrastructure
{
    /// <summary>
    /// Log listener saving to DB and WindowsEvent
    /// </summary>
    public class LogListener : ILogListener
    {
        /// <summary>
        /// The _repository
        /// </summary>
        private readonly IGenericRepository _repository;
        /// <summary>
        /// The event logger
        /// </summary>
        private static readonly ILog EventLogger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of the <see cref="LogListener"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public LogListener(IGenericRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Logs the specified title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="logType">Type of the log.</param>
        /// <param name="entryType">Type of the entry.</param>
        public void Log(string title, LogType logType, LogEntryType entryType)
        {
            var entry = new Log {Title = title, Type = logType};
            LogCreator(entry, entryType);
        }

        /// <summary>
        /// Logs the specified title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="logType">Type of the log.</param>
        /// <param name="entryType">Type of the entry.</param>
        public void Log(string title, string message, LogType logType, LogEntryType entryType)
        {
            var entry = new Log {Title = title, Message = message, Type = logType};
            LogCreator(entry, entryType);
        }

        /// <summary>
        /// Logs the specified title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="otherInfo">The other information.</param>
        /// <param name="logType">Type of the log.</param>
        /// <param name="entryType">Type of the entry.</param>
        public void Log(string title, string message, string otherInfo, LogType logType, LogEntryType entryType)
        {
            var entry = new Log {Title = title, Message = message, OtherInfo = otherInfo, Type = logType};
            LogCreator(entry, entryType);
        }

        /// <summary>
        /// Add timestamp, resolve entry type and save in Eventlog/DB
        /// </summary>
        /// <param name="entry">Log</param>
        /// <param name="entryType">LogEntryType</param>
        private void LogCreator(Log entry, LogEntryType entryType)
        {
            entry.Timestamp = DateTime.Now;

            try
            {
                entry.UserId = (int)HttpContext.Current.Session["UserID"];
            }
            catch { }

            //Create object for EventViewer
            var eViewerEntry = entry.Title;

            if (!string.IsNullOrEmpty(entry.Message))
            {
                eViewerEntry = eViewerEntry + " -  " + entry.Message;
            }
            
            if(!string.IsNullOrEmpty(entry.Information))
            {
                eViewerEntry = eViewerEntry + " -  " + entry.Information;
            }

            if (!string.IsNullOrEmpty(entry.OtherInfo))
            {
                eViewerEntry = eViewerEntry + " -  " + entry.OtherInfo;
            }

            if (entry.UserId != null)
            {
                eViewerEntry = eViewerEntry + " - UserID: " + entry.UserId;
            }
            
            //Log to EventLog according to type
            switch (entryType)
            {
                case LogEntryType.Info:
                    EventLogger.Info(eViewerEntry);
                    break;
                case LogEntryType.Warning:
                    EventLogger.Warn(eViewerEntry);
                    break;
                case LogEntryType.Error:
                    EventLogger.Error(eViewerEntry);
                    break;
            }

            //Log to DB - if fails log to eventviewer
            //EntityOpHandler not used to avoid creating logging loops if DB access not possible
            //TransactionScopeOption.Suppress used to allow error-logging when TransactionScope fails elsewhere
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    _repository.Create(entry); //Log to DB
                    transaction.Complete();
                } 
            }
            catch (Exception ex)
            {
                EventLogger.Fatal(eViewerEntry + " - ExceptionMessage: " + ex.Message);
            }
        }
    }
}