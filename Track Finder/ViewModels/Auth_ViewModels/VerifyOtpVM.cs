using System.ComponentModel.DataAnnotations;

namespace TrackFinder.ViewModels.Auth_ViewModels;

public class VerifyOtpVM
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "OTP code is required.")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digits.")]
    public string OtpCode { get; set; } = string.Empty;
}
