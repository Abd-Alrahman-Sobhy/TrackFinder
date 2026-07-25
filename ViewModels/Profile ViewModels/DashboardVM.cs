using TrackFinder.Models.AssessmentModels;
using TrackFinder.Models.CommunityModels;
using TrackFinder.Models.CourseModels;
using TrackFinder.Models.UserModels;
using TrackFinderDb.Models.TeachingModels;

namespace TrackFinder.ViewModels.Profile_ViewModels
{
    public class DashboardVM
    {
        public User User { get; set; } = null!;
        public string Role { get; set; } = string.Empty;

        public List<Enrollment> EnrolledCourses { get; set; } = new List<Enrollment>();
        public List<Track> RecommendedTracks { get; set; } = new List<Track>();

        public List<Course> CreatedCourses { get; set; } = new List<Course>();
        public int TotalLessonsCount { get; set; }
        public int TotalStudentsCount { get; set; }

        public List<Post> RecentPosts { get; set; } = new List<Post>();

        public List<Community> ManagedCommunities { get; set; } = new();
    }
}
