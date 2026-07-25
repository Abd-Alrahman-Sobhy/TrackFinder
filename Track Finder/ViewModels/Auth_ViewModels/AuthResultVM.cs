namespace TrackFinder.ViewModels.Auth_ViewModels;

public class AuthResultVM
{
    public bool Success { get; set; }
    public string? RedirectUrl { get; set; }
    public string? Message { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
    public bool RequiresEmailVerification { get; set; }
    public string? Email { get; set; }

    public static AuthResultVM Ok(string redirectUrl, string? message = null) =>
        new() { Success = true, RedirectUrl = redirectUrl, SuccessMessage = message, Message = message };

    public static AuthResultVM Fail(string message) =>
        new() { Success = false, ErrorMessage = message, Message = message };

    public static AuthResultVM UnverifiedEmail(string email) =>
        new() { Success = false, RedirectUrl = $"/Login/VerifyOtp?email={Uri.EscapeDataString(email)}", Message = "Please verify your email first.", RequiresEmailVerification = true, Email = email };
}
