namespace TrackFinder.ViewModels.Assessment_ViewModels
{
    public class TrackDetailsVM
    {
        public int Id { get; set; }
        public string TrackName { get; set; } = string.Empty;
        public string TrackDescription { get; set; } = string.Empty;
        public string? RoadMapUrl { get; set; }
        public List<TrackStackVM> Stacks { get; set; } = new();
        public List<GainedSkillVM> GainedSkills { get; set; } = new();
    }

    public class ToolVM
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class TrackStackVM
    {
        public int Id { get; set; }
        public string StackName { get; set; } = string.Empty;
        public string StackDescription { get; set; } = string.Empty;
        public List<ToolVM> Tools { get; set; } = new();
    }

    public class GainedSkillVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}