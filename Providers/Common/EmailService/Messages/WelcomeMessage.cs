namespace TrackFinder.Providers.Common.EmailService.Messages
{
    /// <summary>
    /// Builds the HTML body for the welcome email sent after account activation.
    /// </summary>
    public static class WelcomeMessage
    {
        public static string Build(string firstName, string role)
        {
            var roleMessage = role == "Instructor"
                ? "Your email has been verified. Your account is now pending admin review. You'll receive a notification once approved."
                : "Your account is now active. Start exploring your personalized CS learning path today!";

            var ctaText   = role == "Instructor" ? "Check Your Status" : "Start Learning";
            var ctaColor  = "#006191";

            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
  <title>Welcome to Track Finder</title>
</head>
<body style=""margin:0;padding:0;background:#f0f6ff;font-family:'Segoe UI',Arial,sans-serif;"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#f0f6ff;padding:40px 0;"">
    <tr>
      <td align=""center"">
        <table width=""560"" cellpadding=""0"" cellspacing=""0""
               style=""background:#ffffff;border-radius:16px;overflow:hidden;
                      box-shadow:0 4px 24px rgba(13,43,68,0.10);"">

          <!-- Header -->
          <tr>
            <td style=""background:linear-gradient(135deg,#0061a8 0%,#007bb6 100%);
                        padding:32px 40px;text-align:center;"">
              <h1 style=""margin:0;color:#ffffff;font-size:24px;font-weight:700;"">Track Finder</h1>
              <p style=""margin:4px 0 0;color:#cce7ff;font-size:14px;"">CS Learning Portal</p>
            </td>
          </tr>

          <!-- Body -->
          <tr>
            <td style=""padding:40px 40px 32px;"">
              <h2 style=""margin:0 0 8px;color:#0b1c30;font-size:22px;font-weight:600;"">
                Welcome aboard, {firstName}! 🎉
              </h2>
              <p style=""margin:0 0 24px;color:#3f4850;font-size:16px;line-height:1.6;"">
                {roleMessage}
              </p>
              <div style=""text-align:center;margin:32px 0;"">
                <a href=""#"" style=""background:{ctaColor};color:#ffffff;text-decoration:none;
                              padding:14px 36px;border-radius:8px;font-size:15px;
                              font-weight:600;display:inline-block;"">{ctaText}</a>
              </div>
              <p style=""margin:0;color:#6f7881;font-size:14px;line-height:1.6;"">
                If you have any questions, just reply to this email — we're here to help.
              </p>
            </td>
          </tr>

          <!-- Footer -->
          <tr>
            <td style=""background:#f8f9ff;padding:20px 40px;border-top:1px solid #dce9ff;
                        text-align:center;"">
              <p style=""margin:0;color:#6f7881;font-size:13px;"">
                &copy; {DateTime.UtcNow.Year} Track Finder Inc. All rights reserved.
              </p>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>";
        }
    }
}
