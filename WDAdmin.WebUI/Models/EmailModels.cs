using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WDAdmin.WebUI.Models
{
    /// <summary>
    /// Class EmailNewUserModel.
    /// </summary>
    public class EmailNewUserModel
    {
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        public string Email { get; set; }
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; }
    }

    /// <summary>
    /// Class ForgottenPassEmailModel.
    /// </summary>
    public class ForgottenPassEmailModel
    {
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        public string Email { get; set; }
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; }
    }

    /// <summary>
    /// Class ForgottenPassEmailResponsibleModel.
    /// </summary>
    public class ForgottenPassEmailResponsibleModel
    {
        /// <summary>
        /// Gets or sets the firstname.
        /// </summary>
        /// <value>The firstname.</value>
        public string Firstname { get; set; }
        /// <summary>
        /// Gets or sets the lastname.
        /// </summary>
        /// <value>The lastname.</value>
        public string Lastname { get; set; }
        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        /// <value>The name of the group.</value>
        public string GroupName { get; set; }
        /// <summary>
        /// Gets or sets the salary number.
        /// </summary>
        /// <value>The salary number.</value>
        public int? SalaryNumber { get; set; }
        /// <summary>
        /// Gets or sets the new password.
        /// </summary>
        /// <value>The new password.</value>
        public string NewPassword { get; set; }
    }

    public class QrCodeEmailModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
    }
}