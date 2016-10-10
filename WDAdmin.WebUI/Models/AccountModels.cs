using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WDAdmin.Domain;
using WDAdmin.Resources;
using DataAnnotationsExtensions;
using WDAdmin.Domain.Entities;
using System.Collections.Generic;
using WDAdmin.WebUI.Infrastructure.CustomAttributes;

namespace WDAdmin.WebUI.Models
{
    /// <summary>
    /// Model for login page
    /// </summary>
    public class LogOnModel
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        [Required(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "RequiredFieldError")]
        [Display(ResourceType = typeof(LangResources), Name = "Username")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        [Required(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "RequiredFieldError")]
        [Display(ResourceType = typeof(LangResources), Name = "Password")]
        public string Password { get; set; }
    }

    /// <summary>
    /// Model for self-registration screen
    /// </summary>
    public class RegisterModel
    {
        [Display(ResourceType = typeof(LangResources), Name = "Reference")]
        public string Reference { get; set; }

        public int UserGroupId { get; set; }

        [Required(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "RequiredFieldError")]
        [Integer(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "NumberInputRequiredError")]
        [Display(ResourceType = typeof(LangResources), Name = "SalaryNumber")]
        public int SalaryNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "RequiredFieldError")]
        [Display(ResourceType = typeof(LangResources), Name = "Firstname")]
        public string Firstname { get; set; }

        [Required(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "RequiredFieldError")]
        [Display(ResourceType = typeof(LangResources), Name = "Lastname")]
        public string Lastname { get; set; }

        [Required(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "RequiredFieldError")]
        [Display(ResourceType = typeof(LangResources), Name = "Username")]
        [StringLength(100, ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "RequiredUsernameLengthError", MinimumLength = 5)]
        public string Username { get; set; }

        [Required(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "RequiredFieldError")]
        [StringLength(100, ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "RequiredPasswordLengthError", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(LangResources), Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(LangResources), Name = "ConfirmPassword")]
        [Compare("Password", ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "ConfirmPasswordError")]
        public string ConfirmPassword { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Email")]
        //[CustomRegex(typeof(LangResources), "EmailFormatRegex", ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "EmailFormatError")]
        [LocalizedEmailAddress(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "EmailFormatError")]
        public string Email { get; set; }
    }

    /// <summary>
    /// Model for change password page
    /// </summary>
    public class ChangePasswordModel
    {       
        [Required(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "RequiredFieldError")]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(LangResources), Name = "CurrentPassword")]
        public string OldPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "RequiredFieldError")]
        [StringLength(100, ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "RequiredPasswordLengthError", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(LangResources), Name = "NewPassword")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(LangResources), Name = "ConfirmPassword")]
        [Compare("NewPassword", ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "ConfirmPasswordError")]
        public string ConfirmPassword { get; set; }
    }

    /// <summary>
    /// Model for personal information page
    /// </summary>
    public class UserPersonalViewModel : UserFormModel
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "SalaryNumber")]
        public new int SalaryNumber { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Language")]
        public string Language { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Email")]
        public new string Email { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Username")]
        public new string Username { get; set; }
    }

    /// <summary>
    /// Model for forgotten poassword page
    /// </summary>
    public class ForgottenPasswordModel
    {
        [Required(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "RequiredFieldError")]
        [Display(ResourceType = typeof(LangResources), Name = "Email")]
        public string UserIdent { get; set; }

        public List<GroupsResponsible> ResponsibleUsers { get; set; }
        public int UserId { get; set; }
        public int UserGroupId { get; set; }
        public int ChosenResponsibleId { get; set; }
    }
}