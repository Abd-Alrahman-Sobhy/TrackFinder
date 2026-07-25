using System.ComponentModel.DataAnnotations;

namespace TrackFinder.ViewModels.Auth_ViewModels;

public class ForgotPasswordVM
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;
}
