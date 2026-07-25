namespace TrackFinder.Providers.Common.EmailService.Messages
{
    /// <summary>
    /// Builds the HTML body for the OTP verification email.
    /// </summary>
    public static class OtpMessage
    {
        public static string Build(string firstName, string otpCode, int expiryMinutes = 10)
        {
            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
  <title>Verify Your Email — Track Finder</title>
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
              <h1 style=""margin:0;color:#ffffff;font-size:24px;font-weight:700;
                          letter-spacing:-0.5px;"">Track Finder</h1>
              <p style=""margin:4px 0 0;color:#cce7ff;font-size:14px;"">CS Learning Portal</p>
            </td>
          </tr>

          <!-- Body -->
          <tr>
            <td style=""padding:40px 40px 32px;"">
              <h2 style=""margin:0 0 8px;color:#0b1c30;font-size:22px;font-weight:600;"">
                Verify Your Email Address
              </h2>
              <p style=""margin:0 0 24px;color:#3f4850;font-size:16px;line-height:1.6;"">
                Hi {firstName}, welcome to Track Finder! Use the code below to verify your email.
                The code expires in <strong>{expiryMinutes} minutes</strong>.
              </p>

              <!-- OTP Box -->
              <div style=""background:#e5eeff;border:2px dashed #007bb6;border-radius:12px;
                          padding:24px;text-align:center;margin-bottom:28px;"">
                <p style=""margin:0 0 4px;color:#46617c;font-size:13px;font-weight:500;
                           letter-spacing:1px;text-transform:uppercase;"">Your verification code</p>
                <p style=""margin:0;color:#006191;font-size:42px;font-weight:700;
                           letter-spacing:10px;font-family:'Courier New',monospace;"">{otpCode}</p>
              </div>

              <p style=""margin:0 0 8px;color:#6f7881;font-size:14px;line-height:1.6;"">
                If you didn't create a Track Finder account, you can safely ignore this email.
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
