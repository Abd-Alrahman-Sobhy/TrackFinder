using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace TrackFinder.Providers.Common.EmailService
{
    /// <summary>
    /// Sends emails via SMTP using MailKit.
    /// MailKit correctly handles Gmail's STARTTLS on port 587 — unlike the
    /// obsolete System.Net.Mail.SmtpClient which silently fails with modern SMTP servers.
    /// Configure credentials in appsettings.json under "Email".
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration      _config;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration config, ILogger<EmailSender> logger)
        {
            _config = config;
            _logger = logger;
        }

        /// <inheritdoc/>
        public void SendEmail(string toEmail, string htmlBody, string subject = "Track Finder Notification")
        {
            var smtpHost    = _config["Email:SmtpHost"]    ?? throw new InvalidOperationException("Email:SmtpHost not configured.");
            var smtpPort    = int.Parse(_config["Email:SmtpPort"] ?? "587");
            var fromAddress = _config["Email:FromAddress"] ?? throw new InvalidOperationException("Email:FromAddress not configured.");
            var fromName    = _config["Email:FromName"]    ?? "Track Finder";
            var username    = _config["Email:Username"]    ?? throw new InvalidOperationException("Email:Username not configured.");
            var password    = _config["Email:Password"]    ?? throw new InvalidOperationException("Email:Password not configured.");

            try
            {
                // ── Build the MIME message ────────────────────────────────────
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, fromAddress));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = subject;

                message.Body = new TextPart("html")
                {
                    Text = htmlBody
                };

                // ── Connect, authenticate, send ───────────────────────────────
                using var smtp = new SmtpClient();

                // StartTls negotiates TLS after connecting on port 587.
                // This is what Gmail (and most modern SMTP servers) require.
                smtp.Connect(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                smtp.Authenticate(username, password);
                smtp.Send(message);
                smtp.Disconnect(true);

                _logger.LogInformation("Email sent to {Email} | Subject: {Subject}", toEmail, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                // Don't rethrow — email failure must not crash the registration / OTP flow.
                // The user can always click "Resend Code" to retry.
            }
        }
    }
}
