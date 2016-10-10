using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using ActionMailer.Net.Mvc;
using log4net;
using WDAdmin.Domain.Entities;
using WDAdmin.Resources;
using WDAdmin.WebUI.Infrastructure.Mail;
using WDAdmin.WebUI.Models;

namespace WDAdmin.WebUI.Controllers
{
    /// <summary>
    /// Class EmailController.
    /// </summary>
    public class EmailController : MailerBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(EmailController));

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailController"/> class.
        /// </summary>
        public EmailController()
        {
            MailSender = new LoggingMailSender();
        }

        /// <summary>
        /// Emails the new user.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <returns>EmailResult.</returns>
        public EmailResult EmailNewUser(string email, string password)
        {
            From = AppSettings.AddressFrom;
            Subject = GetLocalizedSubject("PasswordMailSubject");
            To.Add(email);
            return Email("EmailNewUser", new EmailNewUserModel{ Email = email, Password = password });
        }

        /// <summary>
        /// Forgottens the pass email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <returns>EmailResult.</returns>
        public EmailResult ForgottenPassEmail(string email, string password)
        {
            From = AppSettings.AddressFrom;
            Subject = GetLocalizedSubject("NewPasswordMailTitle");
            To.Add(email);
            return Email("ForgottenPassEmail", new ForgottenPassEmailModel{ Email = email, Password = password });
        }

        /// <summary>
        /// Forgottens the pass email responsible.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="user">The user.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>EmailResult.</returns>
        public EmailResult ForgottenPassEmailResponsible(string email, User user, string groupName, string newPassword)
        {
            From = AppSettings.AddressFrom;
            Subject = GetLocalizedSubject("NewPasswordResponsibleMailTitle") + ": " + user.Firstname + " " + user.Lastname;
            To.Add(email);
            return Email("ForgottenPassEmailResponsible", new ForgottenPassEmailResponsibleModel
            {
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                GroupName = groupName,
                SalaryNumber = user.SalaryNumber,
                NewPassword = newPassword,
            });
        }

        /// <summary>
        /// Gets the localized subject.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public string GetLocalizedSubject(string key)
        {
            var culture = AppSettings.DefaultCulture.Name;
            var resource = WDAdmin.WebUI.Infrastructure.ResourceHandler.GetInstance.GetResource(key, culture);
            return resource;
        }

        /// <summary>
        /// Do work after email is sent
        /// </summary>
        /// <param name="mail">MailMessage</param>
        protected override void OnMailSent(MailMessage mail)
        {
            //Log correct mail sendout
            Logger.Info("Mail OK - " + mail.To);
        }
    }
}
