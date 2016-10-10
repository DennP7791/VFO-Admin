using WDAdmin.Domain.Entities;

namespace WDAdmin.WebUI.Infrastructure
{
    /// <summary>
    /// Interface definition for MailListener class
    /// </summary>
    public interface IMailListener
    {
        /// <summary>
        /// Sends the email new user.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        void SendEmailNewUser(string email, string password);
        /// <summary>
        /// Sends the forgotten pass email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="newPassword">The new password.</param>
        void SendForgottenPassEmail(string email, string newPassword);
        /// <summary>
        /// Sends the forgotten pass email responsible.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="user">The user.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="newPassword">The new password.</param>
        void SendForgottenPassEmailResponsible(string email, User user, string groupName, string newPassword);
        /// <summary>
        /// Sends the ok message.
        /// </summary>
        /// <param name="action">The action.</param>
        void SendOKMessage(string action);
        /// <summary>
        /// Sends the error message.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="error">The error.</param>
        void SendErrorMessage(string action, string error);
    }
}