namespace TrackFinder.Models.AssessmentModels
{
    public class TrackSkills
    {
        public int SkillsId { get; set; }
        public int TrackId { get; set; }
        public virtual Track RelatedTrack { get; set; } = null!;
        public virtual GainedSkill GainedSkill { get; set; } = null!;
    }
}
