namespace TrackFinder.ViewModels.Community_ViewModels
{
    public class CommunitySummaryVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MembersCount { get; set; }
        public int OnlineCount { get; set; }
        public bool IsJoined { get; set; }
    }
}
