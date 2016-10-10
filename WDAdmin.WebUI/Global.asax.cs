using System;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using WDAdmin.WebUI.Infrastructure;
using WDAdmin.Domain.Entities;
using System.Configuration;
using WDAdmin.Domain.Abstract;
using WDAdmin.Domain.Concrete;

namespace WDAdmin.WebUI
{
    /// <summary>
    /// Class MvcApplication.
    /// </summary>
    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// The kernel
        /// </summary>
        public static IKernel Kernel = new StandardKernel(new NinjectService());
        /// <summary>
        /// The page structure
        /// </summary>
        private static readonly PageStructureGenerator PageStructure = PageStructureGenerator.GetInstance;

        /// <summary>
        /// Registers the global filters.
        /// </summary>
        /// <param name="filters">The filters.</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routes">RouteCollection object</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");
            routes.IgnoreRoute("robots.txt");
            routes.IgnoreRoute("crossdomain.xml");

            //Route to change culture
            routes.MapRoute(
              "CheckAndChangeCulture", // Route name
              "Account/CheckAndChangeCulture/{culture}", // URL with parameters
              new { controller = "Account", action = "CheckAndChangeCulture", culture = UrlParameter.Optional }
            );

            //Route to get exercises with culture specified
            routes.MapRoute(
              "GetExercises", // Route name
              "Service/GetExercises/{id}/{culture}", // URL with parameters
              new { controller = "Service", action = "GetExercises", culture = UrlParameter.Optional },
              new { id = @"\d+" }
            );

            routes.MapRoute(
               "ExcerciseResults", // Route name
               "VFO/ExcerciseResults/{userId}/{categoryId}", // URL with parameters
               new { controller = "VFO", action = "ExerciseResults", userId = UrlParameter.Optional, categoryId = UrlParameter.Optional }
            );

            routes.MapRoute(
               "SetResponsibleForGroup", // Route name
               "VFO/SetResponsibleForGroup/{groupId}/{responsibleId}", // URL with parameters
               new { controller = "VFO", action = "SetResponsibleForGroup", groupId = UrlParameter.Optional, responsibleId = UrlParameter.Optional }
            );

            routes.MapRoute(
               "ChooseResponsibleUser", // Route name
               "Account/ChooseResponsibleUser/{userId}/{groupId}", // URL with parameters
               new { controller = "Account", action = "ChooseResponsibleUser", userId = UrlParameter.Optional, groupId = UrlParameter.Optional }
            );
            
            routes.MapRoute(
               "Default", // Route name
               "{controller}/{action}/{id}", // URL with parameters
               new { controller = "Account", action = "LogOn", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        /// <summary>
        /// Perform on application start
        /// </summary>
        protected void Application_Start()
        {          
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            //Generate dictionary with structure of existing pages
            PageStructure.GeneratePageStructure();
            
            //Set-up NinjectControllerFactory 
            ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory(Kernel));
 
            //Init log4net
            log4net.Config.XmlConfigurator.Configure();
            Error += Application_Error;

            //Set-up log listener
            var connectionString = ConfigurationManager.ConnectionStrings["VFO"].ConnectionString;
            IDataContextProvider dcp = new DbDataContextProvider(connectionString);
            var repository = new SqlGenericRepository(dcp);
            var logListener = new LogListener(repository);
            Logger.AddLogListener(logListener);

            ////Setup mail listener
            //var mailListener = new MailListener();
            //Mailer.AddMailListener(mailListener);
        }

        /// <summary>
        /// Handles the Error event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Application_Error(object sender, EventArgs e)
        {
            var error = this.Server.GetLastError();
            Logger.Log("Uncaught Exception", error.Message, LogType.UncaughtException, LogEntryType.Error);
            Response.Redirect("/Account/Error");
        }
    }
}