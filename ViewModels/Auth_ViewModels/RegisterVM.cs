using Microsoft.AspNetCore.Http;
using TrackFinder.Models.UserModels;

namespace TrackFinder.ViewModels.Auth_ViewModels;

public class RegisterVM
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string Password { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public DateTime Birthdate { get; set; }
    public string Role { get; set; } = "Student";
    public IFormFile? CvFile { get; set; }
    public string ConfirmPassword { get; set; } = string.Empty;
}
