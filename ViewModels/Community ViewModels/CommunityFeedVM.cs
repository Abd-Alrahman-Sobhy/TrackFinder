using Microsoft.AspNetCore.Mvc.Rendering;

namespace TrackFinder.ViewModels.Community_ViewModels;

public class CommunityFeedVM
{
    public string ActiveFilter { get; set; } = "all";
    public Guid? SelectedCommunityId { get; set; }
    public Guid? CurrentUserId { get; set; }
    public Guid? CurrentStudentId { get; set; }
    public PostFormVM NewPost { get; set; } = new();
    public IReadOnlyList<PostCardVM> Posts { get; set; } = [];
    public IReadOnlyList<CommunitySummaryVM> Communities { get; set; } = [];
    public IReadOnlyList<TopicSummaryVM> TrendingTopics { get; set; } = [];
    public IReadOnlyList<SelectListItem> CommunityOptions { get; set; } = [];
}



