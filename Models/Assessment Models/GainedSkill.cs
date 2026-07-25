using TrackFinder.Models.CourseModels;

namespace TrackFinder.Models.AssessmentModels
{
    public class GainedSkill
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public virtual ICollection<TrackSkills> TrackSkills { get; set; } = null!;
        public virtual ICollection<CourseSkill>? Courses { get; set; } = null!;
    }
}
