using TrackFinder.Models.AssessmentModels;

namespace TrackFinder.Models.CourseModels
{
    public class CourseSkill
    {
        public Guid CourseId { get; set; }
        public int GainedSkillId { get; set; }
        public Course Course { get; set; } = null!;
        public GainedSkill GainedSkill { get; set; } = null!;
    }
}