using System.Collections.Generic;
using System.Web.Mvc;
using WDAdmin.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using WDAdmin.Resources;
using WDAdmin.Domain;
using DataAnnotationsExtensions;
using WDAdmin.WebUI.Infrastructure.CustomAttributes;

namespace WDAdmin.WebUI.Models
{
    /// <summary>
    /// Model for visibility of UserIndexView
    /// </summary>
    public class UserViewModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether [no users].
        /// </summary>
        /// <value><c>true</c> if [no users]; otherwise, <c>false</c>.</value>
        public bool NoUsers { get; set; }

        /// <summary>
        /// Gets or sets the user groups.
        /// </summary>
        /// <value>The user groups.</value>
        [Display(ResourceType = typeof(LangResources), Name = "UserGroup")]
        public List<UserGroup> UserGroups { get; set; }

        /// <summary>
        /// Gets or sets the group identifier.
        /// </summary>
        /// <value>The group identifier.</value>
        [Required(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "GroupForUsersNotChosenError")]
        public int GroupId { get; set; }

        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        /// <value>The users.</value>
        public List<User> Users { get; set; }
    }

    /// <summary>
    /// Model for user create view
    /// </summary>
    public class UserFormModel
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>The user identifier.</value>
        public int UserId { get; set; }
        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        /// <value>The customer identifier.</value>
        public int CustomerId { get; set; }
        /// <summary>
        /// Gets or sets the country identifier.
        /// </summary>
        /// <value>The country identifier.</value>
        public int CountryId { get; set; }

        /// <summary>
        /// Gets or sets the user group identifier.
        /// </summary>
        /// <value>The user group identifier.</value>
        [Required(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "GroupNotChosenError")]
        public int UserGroupId { get; set; }

        /// <summary>
        /// Gets or sets the user group.
        /// </summary>
        /// <value>The user group.</value>
        [Display(ResourceType = typeof(LangResources), Name = "UserGroup")]
        public List<UserGroup> UserGroup { get; set; }

        //List with available User templates
        /// <summary>
        /// Gets or sets the templates.
        /// </summary>
        /// <value>The templates.</value>
        [Display(ResourceType = typeof(LangResources), Name = "UserGroupTemplates")]
        public List<UserTemplate> Templates { get; set; }

        /// <summary>
        /// Gets or sets the user group template identifier.
        /// </summary>
        /// <value>The user group template identifier.</value>
        [Required(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "UserGroupTemplateNotChosenError")]
        public int UserGroupTemplateId { get; set; }

        /// <summary>
        /// Gets or sets the firstname.
        /// </summary>
        /// <value>The firstname.</value>
        [Required(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "RequiredFieldError")]
        [Display(ResourceType = typeof(LangResources), Name = "Firstname")]
        public string Firstname { get; set; }

        /// <summary>
        /// Gets or sets the lastname.
        /// </summary>
        /// <value>The lastname.</value>
        [Required(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "RequiredFieldError")]
        [Display(ResourceType = typeof(LangResources), Name = "Lastname")]
        public string Lastname { get; set; }

        //[Required(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "RequiredFieldError")]
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        [Display(ResourceType = typeof(LangResources), Name = "Email")]
        //[CustomRegex(typeof(LangResources), "EmailFormatRegex", ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "EmailFormatError")]
        [LocalizedEmailAddress(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "EmailFormatError")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the phone.
        /// </summary>
        /// <value>The phone.</value>
        [Display(ResourceType = typeof(LangResources), Name = "Phone")]
        //[CustomRegex(typeof(LangResources), "PhoneFormatRegex", ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "PhoneFormatError")]
        //[Integer(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "NumberInputRequiredError")]
        //[Range(0, 99999999, ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "PhoneFormatError")]
        [LocalizedValidation("Phone", ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "PhoneFormatError")]
        public int? Phone { get; set; }

        //Fields for VFO user, if edited
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        [Required(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "RequiredFieldError")]
        [Display(ResourceType = typeof(LangResources), Name = "Username")]
        [StringLength(100, ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "RequiredUsernameLengthError", MinimumLength = 5)]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the salary number.
        /// </summary>
        /// <value>The salary number.</value>
        [Required(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "RequiredFieldError")]
        [Integer(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "NumberInputRequiredError")]
        [Display(ResourceType = typeof(LangResources), Name = "SalaryNumber")]
        public int SalaryNumber { get; set; }
    }
}