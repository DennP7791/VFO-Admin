using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WDAdmin.WebUI.Models;

namespace WDAdmin.WebUI.Controllers
{
    /// <summary>
    /// Class DebugController.
    /// </summary>
    public class DebugController : Controller
    {
        /// <summary>
        /// The template model map
        /// </summary>
        Dictionary<string, object> TemplateModelMap = new Dictionary<string, object>() {
            { "EmailNewUser", new EmailNewUserModel {
                    Email = "test@exmaple.com",
                    Password = "P4$$w0rd"
                }
            },
            { "ForgottenPassEmail",
                new ForgottenPassEmailModel {
                    Email = "test@exmaple.com",
                    Password = "P4$$w0rd"
                }
            },
            { "ForgottenPassEmailResponsible",
                new ForgottenPassEmailResponsibleModel {
                    Firstname = "Jens",
                    Lastname = "Jensen",
                    GroupName = "Lokal Centeret",
                    SalaryNumber = 12345,
                    NewPassword = "P4$$w0rd",
                }
            },
        };
        /// <summary>
        /// The supported cultures
        /// </summary>
        string[] SupportedCultures = new string[] {
            "da-DK",
            "nb-NO",
            "sv-SE",
        };

        /// <summary>
        /// Emailses the specified culture.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult Emails(string culture = "da-DK")
        {
            ViewBag.Culture = culture;
            ViewBag.SupportedCultures = SupportedCultures;
            return View(TemplateModelMap.Keys.ToArray());
        }

        /// <summary>
        /// Renders the specified template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult Render(string template, string culture)
        {
            var thread = System.Threading.Thread.CurrentThread;
            thread.CurrentCulture = thread.CurrentUICulture = new CultureInfo(culture);
            Session["WDCulture"] = culture;
            if (!TemplateModelMap.ContainsKey(template)) { return HttpNotFound(); }
            var model = TemplateModelMap[template];
            return View("~/Views/Mail/" + template + ".html.cshtml", model);
        }
    }
}
