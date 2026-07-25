using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackFinder.Context;
using TrackFinder.Models.UserModels;

namespace TrackFinder.Controllers.Admin;

[Authorize]
public class ManageAccountsController : Controller
{
    private readonly AppDbContext _context;

    public ManageAccountsController(AppDbContext context)
    {
        _context = context;
    }

    private async Task<bool> IsAdminAsync()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            return false;
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);
        return user != null && user.Role == UserRole.Admin;
    }


    public async Task<IActionResult> Index()
    {
        if (!await IsAdminAsync()) return RedirectToAction("Index", "Login");
        var users = await _context.Users
            .Include(u => u.Instructor)
            .Include(u => u.Student)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        return View("~/Views/Admin/ManageAccounts/Index.cshtml", users);
    }


    // Ban a user
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ban(Guid id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return NotFound();

        user.IsBanned = true;

        _context.Users.Update(user);

        await _context.SaveChangesAsync();

        TempData["Success"] = $"{user.FirstName} {user.LastName} has been banned.";

        return RedirectToAction(nameof(Index));
    }


    // Unban a user
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unban(Guid id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return NotFound();

        user.IsBanned = false;

        _context.Users.Update(user);

        await _context.SaveChangesAsync();

        TempData["Success"] = $"{user.FirstName} {user.LastName} has been unbanned.";

        return RedirectToAction(nameof(Index));
    }


    // Approve an instructor (AdminApproved = true)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveInstructor(Guid id)
    {
        var instructor = await _context.Instructors
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.UserId == id);

        if (instructor == null)
            return NotFound();

        instructor.AdminApproved = true;

        _context.Instructors.Update(instructor);

        await _context.SaveChangesAsync();

        TempData["Success"] = $"{instructor.User.FirstName} {instructor.User.LastName} has been approved as instructor.";

        return RedirectToAction(nameof(Index));
    }


    // Revoke instructor approval (AdminApproved = false)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RevokeInstructor(Guid id)
    {
        var instructor = await _context.Instructors
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.UserId == id);

        if (instructor == null)
            return NotFound();

        instructor.AdminApproved = false;

        _context.Instructors.Update(instructor);

        await _context.SaveChangesAsync();

        TempData["Success"] = $"{instructor.User.FirstName} {instructor.User.LastName} instructor approval has been revoked.";

        return RedirectToAction(nameof(Index));
    }
}