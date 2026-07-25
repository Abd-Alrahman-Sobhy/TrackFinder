using System.ComponentModel.DataAnnotations;
using TrackFinder.Models.CommunityModels;
using TrackFinder.Models.CourseModels;
namespace TrackFinder.Models.UserModels;
public class Instructor
{
    public string? Title { get; set; }
    public string? GithubLink { get; set; } = string.Empty;
    public string? LinkedInLink { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string? CvFilePath { get; set; }
    public bool AdminApproved { get; set; } = false;

    [Key]
    public Guid UserId { get; set; }
    public Guid? CommunityId { get; set; }
    public virtual User User { get; set; } = null!;
    public virtual ICollection<Community> AdminstratedCommunities { get; set; } = new List<Community>();
    public virtual Community? ModeratedCommunity { get; set; }
    public ICollection<Course>? CreatedCourses { get; set; } = null!;
}