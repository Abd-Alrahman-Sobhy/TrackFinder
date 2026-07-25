using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TrackFinder.ViewModels.TrackStacks
{
    public class TrackStackListVM
    {
        public int Id { get; set; }

        public string StackName { get; set; } = string.Empty;

        public string? StackDescription { get; set; }

        public string RelatedTrackName { get; set; } = string.Empty;

        public int CoursesCount { get; set; }
    }

    public class TrackStackDetailsVM
    {
        public int Id { get; set; }

        public string StackName { get; set; } = string.Empty;

        public string? StackDescription { get; set; }

        public string RelatedTrackName { get; set; } = string.Empty;

        public int CoursesCount { get; set; }
    }

    public class CreateTrackStackVM
    {
        [Required]
        [Display(Name = "Stack Name")]
        public string StackName { get; set; } = string.Empty;


        [Display(Name = "Description")]
        public string? StackDescription { get; set; }


        [Required]
        [Display(Name = "Related Track")]
        public int RelatedTrackId { get; set; }
      
        public IEnumerable<SelectListItem>? Tracks { get; set; }
    }

    public class EditTrackStackVM
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Stack Name")]
        public string StackName { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? StackDescription { get; set; }

        [Required]
        [Display(Name = "Related Track")]
        public int RelatedTrackId { get; set; }
    }

    // ViewModel-suffixed aliases for Razor Views
    public class TrackStackListViewModel : TrackStackListVM { }
    public class TrackStackDetailsViewModel : TrackStackDetailsVM { }
    public class CreateTrackStackViewModel : CreateTrackStackVM { }
    public class EditTrackStackViewModel : EditTrackStackVM { }
}