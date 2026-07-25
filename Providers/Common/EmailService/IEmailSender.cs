namespace TrackFinder.Providers.Common.EmailService
{
    /// <summary>
    /// Abstraction for sending emails — swap SMTP for SendGrid or any other provider
    /// by creating a new implementation without touching call sites.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Sends an HTML email to the given address.
        /// </summary>
        /// <param name="toEmail">Recipient email address.</param>
        /// <param name="htmlBody">Full HTML body of the email.</param>
        /// <param name="subject">Email subject line.</param>
        void SendEmail(string toEmail, string htmlBody, string subject = "Track Finder Notification");
    }
}
