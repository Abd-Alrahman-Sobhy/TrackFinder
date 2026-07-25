using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrackFinder.Models.CommunityModels;
using TrackFinder.Models.UserModels;
using TrackFinder.Context;
using TrackFinder.ViewModels.Communities;

namespace TrackFinder.Controllers.Admin;

[Authorize]
public class CommunitiesController : Controller
{
    private readonly AppDbContext _context;

    public CommunitiesController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        if (!await IsAdminAsync()) return RedirectToAction("Index", "Login");
        var items = await _context.Communities
    .Include(c => c.Admin)
        .ThenInclude(i => i.User)
    .Include(c => c.JoinedMembers)
    .ToListAsync();
        var vm = items.Select(c => new CommunityListViewModel
        {
            Id = c.Id,
            Name = c.Name,
            AdminName = c.Admin != null
    ? c.Admin.User.FirstName + " " + c.Admin.User.LastName
    : null,
            MemberCount = c.JoinedMembers?.Count() ?? 0
        }).ToList();
        return View("~/Views/Admin/Communities/Index.cshtml", vm);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var item = await _context.Communities
    .Include(c => c.Admin)
        .ThenInclude(i => i.User)
    .Include(c => c.JoinedMembers)
    .FirstOrDefaultAsync(c => c.Id == id);
        if (item == null) return NotFound();
        var vm = new CommunityDetailsViewModel
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            AdminName = item.Admin != null
    ? item.Admin.User.FirstName + " " + item.Admin.User.LastName
    : null,
            MemberCount = item.JoinedMembers?.Count() ?? 0
        };
        return View("~/Views/Admin/Communities/Details.cshtml", vm);
    }

    public IActionResult Create()
    {
        ViewBag.Instructors = new SelectList(_context.Instructors.Include(i => i.User).Select(i => new { UserId = i.UserId, Name = i.User.FirstName + " " + i.User.LastName }), "UserId", "Name");
        return View("~/Views/Admin/Communities/Create.cshtml", new CreateCommunityViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCommunityViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Instructors = new SelectList(_context.Instructors.Include(i => i.User).Select(i => new { UserId = i.UserId, Name = i.User.FirstName + " " + i.User.LastName }), "UserId", "Name");
            return View("~/Views/Admin/Communities/Create.cshtml", model);
        }

        var community = new Community
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description,

            
            AdminId = model.AdminId

         
        };

        _context.Communities.Add(community);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Edit(Guid id)
    {
        var item = await _context.Communities.FindAsync(id);

        if (item == null)
            return NotFound();

        var vm = new EditCommunityViewModel
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description
        };

        return View("~/Views/Admin/Communities/Edit.cshtml", vm);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EditCommunityViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            return View("~/Views/Admin/Communities/Edit.cshtml", model);
        }

        var item = await _context.Communities.FindAsync(id);

        if (item == null)
            return NotFound();

        item.Name = model.Name;
        item.Description = model.Description;

        _context.Communities.Update(item);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Delete(Guid id)
    {
        var item = await _context.Communities
    .Include(c => c.JoinedMembers)
    .FirstOrDefaultAsync(c => c.Id == id);
        if (item == null) return NotFound();
        var vm = new CommunityDetailsViewModel
        {
            Id = item.Id,
            Name = item.Name,
            AdminName = null,
            MemberCount = item.JoinedMembers?.Count() ?? 0
        };
        return View("~/Views/Admin/Communities/Delete.cshtml", vm);
    }

    [HttpPost, ActionName("DeleteConfirmed")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var item = await _context.Communities.FindAsync(id);
        if (item == null) return NotFound();
        _context.Communities.Remove(item);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
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
}
