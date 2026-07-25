namespace TrackFinder.Models.AssessmentModels
{
    public class Tools
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public virtual ICollection<StackTools>? RelatedStackTools { get; set; } = null!;
    }
}
