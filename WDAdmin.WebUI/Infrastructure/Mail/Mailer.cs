using System.Collections.Generic;
using WDAdmin.Domain.Entities;

namespace WDAdmin.WebUI.Infrastructure
{
    /// <summary>
    /// Service class for mail listeners
    /// </summary>
    public static class Mailer
    {
        /// <summary>
        /// List of mail listeners
        /// </summary>
        private static List<IMailListener> mailListeners = new List<IMailListener>();

        /// <summary>
        /// Sends the email new user.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        public static void SendEmailNewUser(string email, string password)
        {
            // Send further to mail listeners
            foreach (var mailListener in mailListeners) mailListener.SendEmailNewUser(email, password);
        }

        /// <summary>
        /// Sends the forgotten pass email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="newPassword">The new password.</param>
        public static void SendForgottenPassEmail(string email, string newPassword)
        {
            // Send further to mail listeners
            foreach (var mailListener in mailListeners) mailListener.SendForgottenPassEmail(email, newPassword);
        }

        /// <summary>
        /// Sends the forgotten pass email responsible.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="user">The user.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="newPassword">The new password.</param>
		public static void SendForgottenPassEmailResponsible(string email, User user, string groupName, string newPassword)
		{
			// Send further to mail listeners
			foreach (var mailListener in mailListeners) mailListener.SendForgottenPassEmailResponsible(email, user, groupName, newPassword);
		}

        /// <summary>
        /// Sends the ok message.
        /// </summary>
        /// <param name="action">The action.</param>
        public static void SendOKMessage(string action)
        {
            // Send further to mail listeners
            foreach (var mailListener in mailListeners) mailListener.SendOKMessage(action);
        }

        /// <summary>
        /// Sends the error message.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="error">The error.</param>
        public static void SendErrorMessage(string action, string error)
        {
            // Send further to mail listeners
            foreach (var mailListener in mailListeners) mailListener.SendErrorMessage(action, error);
        }

        /// <summary>
        /// Add listener to the list of listeners
        /// </summary>
        /// <param name="listener">The listener.</param>
        public static void AddMailListener(IMailListener listener)
        {
            mailListeners.Add(listener);
        }

        /// <summary>
        /// Remove listener from the list of listeners
        /// </summary>
        /// <param name="listener">IMailListener</param>
        public static void RemoveMailListener(IMailListener listener)
        {
            if (mailListeners.Contains(listener))
            {
                mailListeners.Remove(listener);
            }
        }
    }
}