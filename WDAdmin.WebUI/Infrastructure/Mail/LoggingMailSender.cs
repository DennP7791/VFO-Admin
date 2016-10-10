using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using ActionMailer.Net;
using log4net;

namespace WDAdmin.WebUI.Infrastructure.Mail
{
    /// <summary>
    /// Class LoggingMailSender.
    /// </summary>
    public class LoggingMailSender : IMailSender
    {
        /// <summary>
        /// Class SmtpException.
        /// </summary>
        class SmtpException : Exception { }

        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LoggingMailSender));

        /// <summary>
        /// The _client
        /// </summary>
        private readonly SmtpClient _client;
        /// <summary>
        /// The _callback
        /// </summary>
        private Action<MailMessage> _callback;

        /// <summary>
        /// Creates a new mail sender based on System.Net.Mail.SmtpClient
        /// </summary>
        public LoggingMailSender() : this(new SmtpClient()) { }

        /// <summary>
        /// Creates a new mail sender based on System.Net.Mail.SmtpClient
        /// </summary>
        /// <param name="client">The underlying SmtpClient instance to use.</param>
        public LoggingMailSender(SmtpClient client)
        {
            _client = client;
        }


        /// <summary>
        /// Sends mail synchronously.
        /// </summary>
        /// <param name="mail">The mail you wish to send.</param>
        public void Send(MailMessage mail)
        {
            try
            {
                _client.Send(mail);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        /// <summary>
        /// Sends mail asynchronously.
        /// </summary>
        /// <param name="mail">The mail you wish to send.</param>
        /// <param name="callback">The callback method to invoke when the send operation is complete.</param>
        public void SendAsync(MailMessage mail, Action<MailMessage> callback)
        {
            _callback = callback;
            _client.SendCompleted += new SendCompletedEventHandler(AsyncSendCompleted);
            try
            {
                _client.SendAsync(mail, mail);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        /// <summary>
        /// Asynchronouses the send completed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.AsyncCompletedEventArgs"/> instance containing the event data.</param>
        private void AsyncSendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            // unsubscribe from the event so _client can be GC'ed if necessary
            _client.SendCompleted -= AsyncSendCompleted;
            if (e.Error != null)
            {
                Logger.Error(e.Error.ToString());
            }
            _callback(e.UserState as MailMessage);
        }

        /// <summary>
        /// Destroys the underlying SmtpClient.
        /// </summary>
        public void Dispose()
        {
            _client.Dispose();
        }
    }
}