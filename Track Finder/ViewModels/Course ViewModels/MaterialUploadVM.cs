using System.ComponentModel.DataAnnotations;

namespace TrackFinder.ViewModels
{
    public class MaterialUploadVM
    {
        public Guid LessonId { get; set; }
        
        [Required(ErrorMessage = "Please select a file to upload.")]
        [Display(Name = "Material File")]
        public IFormFile File { get; set; } = null!;
    }
}
