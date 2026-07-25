using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrackFinder.ViewModels.Auth_ViewModels;
using TrackFinder.ViewModels.Profile_ViewModels;

namespace TrackFinder.Services.UserProfileServices
{
    public interface IUserProfileService
    {
        Task<DashboardVM> GetDashboardDataAsync(Guid userId, string role);
        Task<EditProfileVM> GetProfileForEditAsync(Guid userId, string role);
        Task<AuthResultVM> UpdateProfileAsync(Guid userId, string role, EditProfileVM dto, string webRootPath);
        Task<List<UserSearchResultVM>> SearchUsersAsync(string query, Guid currentUserId);
        Task<PublicProfileVM> GetPublicProfileAsync(Guid userId);
        Task<AuthResultVM> DeleteCommunityPostAsync(Guid postId, Guid instructorUserId);
        Task<AuthResultVM> ApprovePostAsync(Guid postId, Guid instructorUserId);
        Task<AuthResultVM> RejectPostAsync(Guid postId, Guid instructorUserId);
    }
}
