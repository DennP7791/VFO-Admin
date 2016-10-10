using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WDAdmin.Domain.Abstract;
using WDAdmin.WebUI.Infrastructure;
using WDAdmin.Domain.Entities;
using WDAdmin.Resources;
using WDAdmin.WebUI.Models;

namespace WDAdmin.WebUI.Controllers
{
    /// <summary>
    /// Class LogController.
    /// </summary>
    [Authorize]
    public class LogController : BaseController
    {
        /// <summary>
        /// The _repository
        /// </summary>
        private readonly IGenericRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogController"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public LogController(IGenericRepository repository) : base(repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// LogIndex page
        /// </summary>
        /// <returns>LogIndex view</returns>
        [AuthorizeAccessAttribute("Group5Page1")]
        public ActionResult LogIndex()
        {
            var model = new LogViewModel {LogEntries = new List<Log>()};

            //Pick log entries from last 7 days only - sort from the newest down
            var logEntries = (from le in _repository.Get<Log>() where (DateTime.Now.Date - le.Timestamp.Date).TotalDays <= 7 orderby le.Timestamp descending select le).ToList();

            if (!logEntries.Any())
            {
                model.NoLogEntries = true;
                return View(model);
            }

            foreach (var entry in logEntries)
            {
                var lentry = new Log { Id = entry.Id, Time = (entry.Timestamp).ToString("dd/MM/yyyy HH:mm:ss"), Title = entry.Title };

                if (entry.UserId != null)
                {
                    lentry.Information = LangResources.UserId + ": " + entry.UserId.ToString();
                }
                else
                {
                    lentry.Information = entry.OtherInfo;
                }

                model.LogEntries.Add(lentry);
            }

            return View(model);
        }

        /// <summary>
        /// LogDetails page
        /// </summary>
        /// <param name="id">Log entry ID</param>
        /// <returns>LogDetails view</returns>
        [AuthorizeAccessAttribute("Group5Page2")]
        public ActionResult LogDetails(int id)
        {
            var culture = Session["WDCulture"].ToString();
            var model = new LogDetailsModel();

            //Get the correct logpost from DB
            Log logpost;
            try
            {
                logpost = (from lp in _repository.Get<Log>() where lp.Id == id select lp).Single();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("LogpostNotFoundError", culture));
                Logger.Log("LogDetails Error", ex.Message, LogType.DbQueryError, LogEntryType.Error);
                return View(model);
            }

            model.Title = logpost.Title;
            model.Time = (logpost.Timestamp).ToString("dd/MM/yyyy HH:mm:ss");

            if (!string.IsNullOrEmpty(logpost.Message)) //View Message if present
            {
                model.Message = logpost.Message;
            }

            var type = (LogType)logpost.Type;
            model.Type = type.ToString();

            if (logpost.UserId != null) //View UserId if present
            {
                model.Information = LangResources.UserId + ": " + logpost.UserId.ToString();
            }

            if (logpost.UserId != null && logpost.OtherInfo != null) //View UserId + OtherInfo if present
            {
                model.Information = model.Information + " - " + logpost.OtherInfo;
            }
            else if (logpost.UserId == null && logpost.OtherInfo != null) //View OtherInfo if present
            {
                model.Information = logpost.OtherInfo;
            }

            return View(model);
        }
    }
}
