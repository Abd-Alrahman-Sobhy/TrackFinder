using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrackFinder.Context;
using TrackFinder.Models.CommunityModels;
using TrackFinder.Models.UserModels;
using TrackFinder.ViewModels.Community_ViewModels;

namespace TrackFinder.Controllers;

public class CommunityController : Controller
{
    private static readonly Regex HashtagRegex = new(@"#(?<tag>[A-Za-z0-9_]+)", RegexOptions.Compiled);
    private readonly AppDbContext _context;

    public CommunityController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string filter = "all", Guid? communityId = null)
    {
        var currentUser = await GetCurrentUserAsync();
        var currentStudentId = await GetCurrentStudentIdAsync(currentUser?.Id);
        var communities = await BuildCommunitiesAsync(currentStudentId);
        var selectedCommunityId = communityId ?? communities.FirstOrDefault()?.Id;

        var postQuery = _context.Posts
            .AsNoTracking()
            .Include(p => p.Author)
            .Include(p => p.Community)
            .Include(p => p.Comments)
                .ThenInclude(c => c.User)
            .Where(p => p.Status == PostStatus.Approved || p.Status == PostStatus.PendingReview);

        postQuery = filter.ToLowerInvariant() switch
        {
            "instructors" => postQuery.Where(p => p.Author.Role == UserRole.Instructor),
            "study-groups" => postQuery.Where(p => p.Community.JoinedMembers.Any()),
            "course-specific" => selectedCommunityId.HasValue
                ? postQuery.Where(p => p.GroupId == selectedCommunityId.Value)
                : postQuery,
            _ => postQuery
        };

        var posts = await postQuery
            .OrderByDescending(p => p.PostPriority)
            .ThenByDescending(p => p.CreatedAt)
            .ToListAsync();

        var model = new CommunityFeedVM
        {
            ActiveFilter = filter,
            SelectedCommunityId = selectedCommunityId,
            CurrentUserId = currentUser?.Id,
            CurrentStudentId = currentStudentId,
            NewPost = new PostFormVM
            {
                GroupId = selectedCommunityId ?? Guid.Empty
            },
            Communities = communities,
            CommunityOptions = communities.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),
                Selected = c.Id == selectedCommunityId
            }).ToList(),
            Posts = posts.Select(p => ToPostCard(p, currentUser?.Id)).ToList(),
            TrendingTopics = BuildTrendingTopics(posts)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePost(PostFormVM input)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser is null)
        {
            TempData["CommunityMessage"] = "Add a user before posting to the community feed.";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid || !await _context.Communities.AnyAsync(c => c.Id == input.GroupId))
        {
            TempData["CommunityMessage"] = "Choose a community and add post content.";
            return RedirectToAction(nameof(Index));
        }

        _context.Posts.Add(new Post
        {
            Content = input.Content.Trim(),
            ImageUrl = string.IsNullOrWhiteSpace(input.ImageUrl) ? null : input.ImageUrl.Trim(),
            GroupId = input.GroupId,
            UserId = currentUser.Id,
            CreatedAt = DateTime.UtcNow,
            Status = PostStatus.Approved,
            PostPriority = input.PostPriority
        });

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { communityId = input.GroupId });
    }

    public async Task<IActionResult> EditPost(Guid id)
    {
        var post = await _context.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post is null)
        {
            return NotFound();
        }

        var currentUser = await GetCurrentUserAsync();
        if (!CanManagePost(post, currentUser?.Id))
        {
            return Forbid();
        }

        ViewBag.Communities = await GetCommunitySelectListAsync(post.GroupId);

        return View(new PostFormVM
        {
            Id = post.Id,
            Content = post.Content,
            GroupId = post.GroupId,
            ImageUrl = post.ImageUrl,
            PostPriority = post.PostPriority
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost(Guid id, PostFormVM input)
    {
        if (id != input.Id)
        {
            return BadRequest();
        }

        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
        if (post is null)
        {
            return NotFound();
        }

        var currentUser = await GetCurrentUserAsync();
        if (!CanManagePost(post, currentUser?.Id))
        {
            return Forbid();
        }

        if (!ModelState.IsValid || !await _context.Communities.AnyAsync(c => c.Id == input.GroupId))
        {
            ViewBag.Communities = await GetCommunitySelectListAsync(input.GroupId);
            return View(input);
        }

        post.Content = input.Content.Trim();
        post.GroupId = input.GroupId;
        post.ImageUrl = string.IsNullOrWhiteSpace(input.ImageUrl) ? null : input.ImageUrl.Trim();
        post.PostPriority = input.PostPriority;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { communityId = post.GroupId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePost(Guid id)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
        if (post is null)
        {
            return NotFound();
        }

        var currentUser = await GetCurrentUserAsync();
        if (!CanManagePost(post, currentUser?.Id))
        {
            return Forbid();
        }

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(CommentFormVM input)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser is null)
        {
            TempData["CommunityMessage"] = "Add a user before commenting.";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid || !await _context.Posts.AnyAsync(p => p.Id == input.PostId))
        {
            TempData["CommunityMessage"] = "Write a comment before submitting.";
            return RedirectToAction(nameof(Index));
        }

        _context.Comments.Add(new Comment
        {
            PostId = input.PostId,
            ParentCommentId = input.ParentCommentId,
            Content = input.Content.Trim(),
            UserId = currentUser.Id,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteComment(Guid id)
    {
        var comment = await _context.Comments
            .Include(c => c.Replies)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (comment is null)
        {
            return NotFound();
        }

        var currentUser = await GetCurrentUserAsync();
        if (currentUser?.Id != comment.UserId && currentUser?.Role != UserRole.Admin)
        {
            return Forbid();
        }

        if (comment.Replies?.Any() == true)
        {
            _context.Comments.RemoveRange(comment.Replies);
        }

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Join(Guid id)
    {
        var studentId = await GetCurrentStudentIdAsync((await GetCurrentUserAsync())?.Id);
        if (studentId is null)
        {
            TempData["CommunityMessage"] = "Only student accounts can join groups.";
            return RedirectToAction(nameof(Index));
        }

        var exists = await _context.JoinedMembers.AnyAsync(jm => jm.MemberId == studentId && jm.CommunityId == id);
        if (!exists && await _context.Communities.AnyAsync(c => c.Id == id))
        {
            _context.JoinedMembers.Add(new JoinedMembers
            {
                MemberId = studentId.Value,
                CommunityId = id
            });

            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index), new { communityId = id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Report(Guid id, ReportReason reason = ReportReason.Other)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser is null)
        {
            TempData["CommunityMessage"] = "Add a user before reporting a post.";
            return RedirectToAction(nameof(Index));
        }

        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
        if (post is null)
        {
            return NotFound();
        }

        _context.PostReports.Add(new PostReport
        {
            PostId = id,
            ReporterId = currentUser.Id,
            Reason = reason,
            CreatedAt = DateTime.UtcNow
        });

        post.ReportsCount += 1;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    
    private async Task<User?> GetCurrentUserAsync()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            return null;

        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsBanned);
    }

    private async Task<Guid?> GetCurrentStudentIdAsync(Guid? userId)
    {
        if (userId is null)
        {
            return null;
        }

        return await _context.Students
            .Where(s => s.UserId == userId)
            .Select(s => (Guid?)s.UserId)
            .FirstOrDefaultAsync();
    }

    private async Task<IReadOnlyList<CommunitySummaryVM>> BuildCommunitiesAsync(Guid? currentStudentId)
    {
        var communities = await _context.Communities
            .AsNoTracking()
            .Include(c => c.JoinedMembers)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return communities.Select((community, index) => new CommunitySummaryVM
        {
            Id = community.Id,
            Name = community.Name,
            MembersCount = community.JoinedMembers?.Count ?? 0,
            OnlineCount = Math.Max(1, (community.JoinedMembers?.Count ?? 0) / 4 + index),
            IsJoined = currentStudentId.HasValue && (community.JoinedMembers?.Any(jm => jm.MemberId == currentStudentId.Value) ?? false)
        }).ToList();
    }

    private async Task<IReadOnlyList<SelectListItem>> GetCommunitySelectListAsync(Guid selectedCommunityId)
    {
        return await _context.Communities
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),
                Selected = c.Id == selectedCommunityId
            })
            .ToListAsync();
    }

    private static PostCardVM ToPostCard(Post post, Guid? currentUserId)
    {
        return new PostCardVM
        {
            Id = post.Id,
            Content = post.Content,
            ImageUrl = post.ImageUrl,
            CreatedAt = post.CreatedAt,
            Status = post.Status,
            ReportsCount = post.ReportsCount,
            PostPriority = post.PostPriority,
            Author = post.Author,
            Community = post.Community,
            Tags = ExtractTags(post.Content).ToList(),
            Comments = post.Comments
                .Where(c => c.ParentCommentId == null)
                .OrderByDescending(c => c.CreatedAt)
                .Take(2)
                .Select(c => new CommentPreviewVM
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    User = c.User,
                    CanManage = currentUserId.HasValue && c.UserId == currentUserId.Value
                })
                .ToList(),
            CanManage = CanManagePost(post, currentUserId)
        };
    }

    private static bool CanManagePost(Post post, Guid? currentUserId)
    {
        return currentUserId.HasValue && post.UserId == currentUserId.Value;
    }

    private static IReadOnlyList<TopicSummaryVM> BuildTrendingTopics(IEnumerable<Post> posts)
    {
        var topics = posts
            .SelectMany(p => ExtractTags(p.Content))
            .GroupBy(tag => tag, StringComparer.OrdinalIgnoreCase)
            .Select(group => new TopicSummaryVM
            {
                Name = group.Key,
                PostsCount = group.Count()
            })
            .OrderByDescending(topic => topic.PostsCount)
            .ThenBy(topic => topic.Name)
            .Take(4)
            .ToList();

        return topics.Count > 0
            ? topics
            : [new TopicSummaryVM { Name = "DotNet", PostsCount = 0 }];
    }

    private static IEnumerable<string> ExtractTags(string content)
    {
        return HashtagRegex.Matches(content ?? string.Empty)
            .Select(match => match.Groups["tag"].Value)
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Distinct(StringComparer.OrdinalIgnoreCase);
    }
}
