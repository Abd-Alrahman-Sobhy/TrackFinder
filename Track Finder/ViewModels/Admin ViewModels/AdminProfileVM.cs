using System.ComponentModel.DataAnnotations;
using TrackFinder.Models.UserModels;
namespace TrackFinder.ViewModels.Users
{
    public class AdminProfileVM
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Birthdate")]
        public DateTime Birthdate { get; set; }

        [Display(Name = "Gender")]
        public Gender Gender { get; set; }

        [Display(Name = "Bio")]
        public string? Bio { get; set; }

        public string? ProfilePictureUrl { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile? Image { get; set; }
    }
}