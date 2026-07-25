using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TrackFinder.ViewModels.Tracks
{
    public class TrackListVM
    {
        public int Id { get; set; }
        public string TrackName { get; set; } = string.Empty;
        public string TrackDescription { get; set; } = string.Empty;
        public string? RoadMapUrl { get; set; }
    }

    public class CreateTrackVM
    {
        [Required]
        public string TrackName { get; set; } = string.Empty;
        public string? TrackDescription { get; set; }
        public string? RoadMapUrl { get; set; }
    }

    public class EditTrackVM
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string TrackName { get; set; } = string.Empty;
        public string? TrackDescription { get; set; }
        public string? RoadMapUrl { get; set; }
    }
    public class TrackDetailsVM
    {
        public int Id { get; set; }
        public string TrackName { get; set; } = string.Empty;
        public string? TrackDescription { get; set; }
        public string? RoadMapUrl { get; set; }
    }

    // ViewModel-suffixed aliases for Razor Views
    public class TrackListViewModel : TrackListVM { }
    public class CreateTrackViewModel : CreateTrackVM { }
    public class EditTrackViewModel : EditTrackVM { }
    public class TrackDetailsViewModel : TrackDetailsVM { }
}
