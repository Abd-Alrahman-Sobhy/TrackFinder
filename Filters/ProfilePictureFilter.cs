using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TrackFinder.Models.UserModels;

namespace TrackFinder.Filters;

public class ProfilePictureFilter : IAsyncActionFilter
{
    private readonly UserManager<User> _userManager;

    public ProfilePictureFilter(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is not null)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user is not null && context.Controller is Controller controller)
                {
                    controller.ViewBag.ProfilePictureUrl = user.ProfilePictureUrl;
                }
            }
        }
        await next();
    }
}
