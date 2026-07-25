using System.ComponentModel.DataAnnotations;

namespace TrackFinder.ViewModels.Community_ViewModels
{
    public class PostFormVM
    {
        public Guid? Id { get; set; }

        [Required]
        public Guid GroupId { get; set; }

        [Required]
        [Display(Name = "Post")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Image URL")]
        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public int PostPriority { get; set; }
    }
}
