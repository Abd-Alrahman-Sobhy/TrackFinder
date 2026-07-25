using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackFinder.Models.AchievementModels;
using TrackFinder.Context;
using TrackFinder.ViewModels.Achievements;

namespace TrackFinder.Controllers.Admin;

[Authorize]
public class BadgesController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public BadgesController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _context.Badges.ToListAsync();
        var vm = items.Select(b => new BadgeListViewModel
        {
            BadgeId = b.BadgeId,
            BadgeName = b.BadgeName,
            BadgeDescription = b.BadgeDescription,
            BadgeImageUrl = b.BadgeImageUrl
        }).ToList();
        return View("~/Views/Admin/Badges/Index.cshtml", vm);
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await _context.Badges.FindAsync(id);
        if (item == null) return NotFound();
        var vm = new BadgeDetailsViewModel
        {
            BadgeId = item.BadgeId,
            BadgeName = item.BadgeName,
            BadgeDescription = item.BadgeDescription,
            BadgeImageUrl = item.BadgeImageUrl
        };
        return View("~/Views/Admin/Badges/Details.cshtml", vm);
    }

    public IActionResult Create()
    {
        return View("~/Views/Admin/Badges/Create.cshtml", new CreateBadgeViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBadgeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Admin/Badges/Create.cshtml", model);
        }

        var badge = new Badges { BadgeName = model.BadgeName, BadgeDescription = model.BadgeDescription };
        if (model.Image != null && model.Image.Length > 0)
        {
            var uploads = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploads);
            var fileName = $"badge_{Guid.NewGuid()}{Path.GetExtension(model.Image.FileName)}";
            var filePath = Path.Combine(uploads, fileName);
            using (var stream = System.IO.File.Create(filePath))
            {
                await model.Image.CopyToAsync(stream);
            }
            badge.BadgeImageUrl = $"/uploads/{fileName}";
        }

        _context.Badges.Add(badge);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _context.Badges.FindAsync(id);
        if (item == null) return NotFound();
        var vm = new EditBadgeViewModel
        {
            BadgeId = item.BadgeId,
            BadgeName = item.BadgeName,
            BadgeDescription = item.BadgeDescription
        };
        return View("~/Views/Admin/Badges/Edit.cshtml", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditBadgeViewModel model)
    {
        if (id != model.BadgeId) return BadRequest();
        if (!ModelState.IsValid)
        {
            return View("~/Views/Admin/Badges/Edit.cshtml", model);
        }

        var item = await _context.Badges.FindAsync(id);
        if (item == null) return NotFound();
        item.BadgeName = model.BadgeName;
        item.BadgeDescription = model.BadgeDescription;
        if (model.Image != null && model.Image.Length > 0)
        {
            var uploads = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploads);
            var fileName = $"badge_{Guid.NewGuid()}{Path.GetExtension(model.Image.FileName)}";
            var filePath = Path.Combine(uploads, fileName);
            using (var stream = System.IO.File.Create(filePath))
            {
                await model.Image.CopyToAsync(stream);
            }
            item.BadgeImageUrl = $"/uploads/{fileName}";
        }

        _context.Badges.Update(item);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Badges.FindAsync(id);
        if (item == null) return NotFound();
        var vm = new BadgeDetailsViewModel
        {
            BadgeId = item.BadgeId,
            BadgeName = item.BadgeName,
            BadgeDescription = item.BadgeDescription,
            BadgeImageUrl = item.BadgeImageUrl
        };
        return View("~/Views/Admin/Badges/Delete.cshtml", vm);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int badgeId)
    {
        var item = await _context.Badges.FindAsync(badgeId);

        if (item == null)
            return NotFound();

        _context.Badges.Remove(item);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
