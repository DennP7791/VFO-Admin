using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WDAdmin.Domain.Entities;

namespace WDAdmin.WebUI.Infrastructure
{
    /// <summary>
    /// Class AuthorizeAccessAttribute. Custom authorization attribute checking authorization via login & via page rights access 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AuthorizeAccessAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Gets or sets the page identifier.
        /// </summary>
        /// <value>The page identifier.</value>
        private string PageId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeAccessAttribute"/> class.
        /// </summary>
        /// <param name="pageId">The page identifier.</param>
        public AuthorizeAccessAttribute(string pageId)
        {
            PageId = pageId;
        }

        /// <summary>
        /// Called when [action executing].
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity == null || !filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                LogOut(filterContext, "UnauthenticatedAccessAttempt", LogType.UnauthenticatedAccess);
            }
            else //User is auhorized via login - check access rights
            {
                if (PageId != "CustomPageAuthorize") //Check if page is not custom authorized
                {
                    //Get UserGroup rights from Session
                    var rights = filterContext.HttpContext.Session["Rights"];

                    if (rights == null)
                    {
                        LogOut(filterContext, "Session/Rights is null", LogType.SessionExpired);
                    }

                    //Find the property with side name and get its value
                    var modelType = rights.GetType();
                    var rightInfo = modelType.GetProperty(PageId);
                    var rightValue = (bool)rightInfo.GetValue(rights, null);

                    //If user not authorized to see page - reset Sesion variables, log access attempt and redirect to Error page
                    if (!rightValue)
                    {
                        LogOut(filterContext, "UnauthorizedAccessAttempt", LogType.UnauthorizedAccess);
                    }
                }
            }
        }

        /// <summary>
        /// Log out from the system -&gt; nullify session values, cookies and redirect to logon page
        /// </summary>
        /// <param name="filterContext">Action context</param>
        /// <param name="logMessage">Log entry message</param>
        /// <param name="logType">Log entry type</param>
        private void LogOut(ActionExecutingContext filterContext, string logMessage, LogType logType)
        {
            //Log entry
            Logger.Log(logMessage, logType, LogEntryType.Warning);

			//Prevent caching the page so users cannot use the back button when they log-out
			filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
			filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			
			//Clear Session variables and abandon
            filterContext.HttpContext.Session.Clear();
            filterContext.HttpContext.Session.Abandon();

            //Clean auth cookie 
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, string.Empty) { Expires = DateTime.Now.AddDays(-1) };
            filterContext.HttpContext.Response.Cookies.Add(authCookie);

			//Clean session cookie 
			var sessionCookie = filterContext.HttpContext.Request.Cookies["ASP.NET_SessionId"];
			sessionCookie.Value = string.Empty;
			sessionCookie.Expires = DateTime.Now.AddDays(-1);
			filterContext.HttpContext.Response.Cookies.Set(sessionCookie);

            FormsAuthentication.SignOut(); //Signout
            filterContext.HttpContext.Session.Abandon(); //Abandon Session

            //Redirect to login page
            filterContext.Result = new RedirectResult("/Account/LogOn");
        }
    } 
}