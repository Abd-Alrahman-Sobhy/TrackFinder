using System.ComponentModel.DataAnnotations;
using TrackFinder.Models.UserModels;

namespace TrackFinder.ViewModels.Users
{
    public class UserListVM
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateUserVM
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }

        public DateTime Birthdate { get; set; }

        [Required]
        public Gender Gender { get; set; }

        public string? Bio { get; set; }

        public IFormFile? Image { get; set; }

        public bool EmailVerified { get; set; }
        public bool IsBanned { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }
    }

    public class EditUserVM
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }

        public DateTime Birthdate { get; set; }
        public Gender Gender { get; set; }
        public string? Bio { get; set; }
        public IFormFile? Image { get; set; }
        public bool EmailVerified { get; set; }
        public bool IsBanned { get; set; }

        [StringLength(100, MinimumLength = 6)]
        public string? Password { get; set; }

        [Required]
        public UserRole Role { get; set; }
    }

    public class UserDetailsVM
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime Birthdate { get; set; }
        public Gender Gender { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public bool EmailVerified { get; set; }
        public bool IsBanned { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserRole Role { get; set; }
    }

    // ViewModel-suffixed aliases for Razor Views
    public class UserListViewModel : UserListVM { }
    public class CreateUserViewModel : CreateUserVM { }
    public class EditUserViewModel : EditUserVM { }
    public class UserDetailsViewModel : UserDetailsVM { }
}
