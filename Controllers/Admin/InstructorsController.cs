using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrackFinder.Models.UserModels;
using TrackFinder.Context;
using TrackFinder.ViewModels.Instructors;

namespace TrackFinder.Controllers.Admin;

[Authorize]
public class InstructorsController : Controller
{
    private readonly AppDbContext _context;

    public InstructorsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _context.Instructors.Include(i => i.User).ToListAsync();
        return View("~/Views/Admin/Instructors/Index.cshtml", items);
    }

    public async Task<IActionResult> Details(Guid userId)
    {
        var item = await _context.Instructors
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.UserId == userId);

        if (item == null)
            return NotFound();

        var vm = new InstructorDetailsViewModel
        {
            UserId = item.UserId,
            InstructorName = item.User.FirstName + " " + item.User.LastName,
            Title = item.Title,
            GithubLink = item.GithubLink,
            LinkedInLink = item.LinkedInLink,
            Rating = item.Rating
        };

        return View("~/Views/Admin/Instructors/Details.cshtml", vm);
    }
    public IActionResult Create()
    {
        ViewBag.Users = new SelectList(
            _context.Users.Select(u => new
            {
                u.Id,
                Name = u.FirstName + " " + u.LastName
            }),
            "Id",
            "Name"
        );

        ViewBag.Communities = new SelectList(
            _context.Communities.Select(c => new
            {
                c.Id,
                c.Name
            }),
            "Id",
            "Name"
        );

        return View(
            "~/Views/Admin/Instructors/Create.cshtml",
            new CreateInstructorViewModel()
        );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateInstructorViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Users = new SelectList(
                _context.Users.Select(u => new
                {
                    u.Id,
                    Name = u.FirstName + " " + u.LastName
                }),
                "Id",
                "Name",
                model.UserId
            );

            ViewBag.Communities = new SelectList(
                _context.Communities.Select(c => new
                {
                    c.Id,
                    c.Name
                }),
                "Id",
                "Name",
                model.CommunityId
            );

            return View("~/Views/Admin/Instructors/Create.cshtml", model);
        }

        var instructor = new Instructor
        {
            UserId = model.UserId,
            Title = model.Title,
            GithubLink = model.GithubLink,
            LinkedInLink = model.LinkedInLink,
            Rating = model.Rating,
            CommunityId = model.CommunityId
        };

        _context.Instructors.Add(instructor);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid userId)
{
    var item = await _context.Instructors.FindAsync(userId);

    if (item == null)
        return NotFound();

    ViewBag.Users = new SelectList(
        _context.Users.Select(u => new { u.Id, Name = u.FirstName + " " + u.LastName }),
        "Id",
        "Name",
        item.UserId);

   

    var model = new EditInstructorViewModel
    {
        UserId = item.UserId,
        Title = item.Title,
        GithubLink = item.GithubLink,
        LinkedInLink = item.LinkedInLink,
        Rating = item.Rating
    };

    return View("~/Views/Admin/Instructors/Edit.cshtml", model);
}

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid userId, EditInstructorViewModel model)
    {
        if (userId != model.UserId) return BadRequest();
        if (!ModelState.IsValid)
        {
            ViewBag.Users = new SelectList(
                _context.Users.Select(u => new { u.Id, Name = u.FirstName + " " + u.LastName }),
                "Id",
                "Name",
                model.UserId);

           

            return View("~/Views/Admin/Instructors/Edit.cshtml", model);
        }

        var item = await _context.Instructors.FindAsync(userId);
        if (item == null) return NotFound();
        item.Title = model.Title;
        item.GithubLink = model.GithubLink;
        item.LinkedInLink = model.LinkedInLink;
        item.Rating = model.Rating;
        _context.Instructors.Update(item);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid userId)
    {
        var instructor = await _context.Instructors
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.UserId == userId);

        if (instructor == null)
            return NotFound();

        var model = new InstructorDetailsVM
        {
            UserId = instructor.UserId,
            InstructorName = instructor.User.FirstName + " " + instructor.User.LastName,
            Title = instructor.Title,
            GithubLink = instructor.GithubLink,
            LinkedInLink = instructor.LinkedInLink,
            Rating = instructor.Rating
        };

        return View("~/Views/Admin/Instructors/Delete.cshtml", model);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid userId)
    {
        var instructor = await _context.Instructors
            .FirstOrDefaultAsync(i => i.UserId == userId);

        if (instructor == null)
            return NotFound();

        _context.Instructors.Remove(instructor);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
