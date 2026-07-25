using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrackFinder.Models.CourseModels;
using TrackFinder.ViewModels.Courses;
using TrackFinder.Context;

namespace TrackFinder.Controllers.Admin;

[Authorize]
public class CoursesController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public CoursesController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var courses = await _context.Courses
            .Include(c => c.Instructor)
            .ThenInclude(i => i.User)
            .Select(c => new CourseListViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Price = c.Price,
                Level = c.Level,
                InstructorName = c.Instructor != null
                    ? c.Instructor.User.FirstName + " " + c.Instructor.User.LastName
                    : ""
            })
            .ToListAsync();

        return View("~/Views/Admin/Courses/Index.cshtml", courses);
    }

    public async Task<IActionResult> Details(Guid id)
{
        var course = await _context.Courses
     .Include(c => c.Instructor)
         .ThenInclude(i => i.User)
     .Include(c => c.TrackStack)
     .FirstOrDefaultAsync(c => c.Id == id);
        if (course == null)
        return NotFound();

        var model = new CourseDetailsViewModel
        {
            Id = course.Id,
            Name = course.Name,
            Description = course.Description,
            Price = course.Price,
            ImageUrl = course.ImageUrl,
            Language = course.Language,
            Level = course.Level,
            Rating = course.Rating,
            Discount = course.Discount,

            DurationValue = course.Duration != null
                ? course.Duration.Value
                : 0,

            DurationIn = course.Duration != null
                ? course.Duration.DurationIn
                : DurationIn.Days,

            InstructorName = course.Instructor?.User != null
    ? course.Instructor.Title + " " +
      course.Instructor.User.FirstName + " " +
      course.Instructor.User.LastName
    : "No Instructor",
            TrackStackName = course.TrackStack != null
                ? course.TrackStack.StackName
                : "No Track Stack"
        };
        return View("~/Views/Admin/Courses/Details.cshtml", model);
}
    public IActionResult Create()
    {
        PopulateSelectLists();

        return View(
            "~/Views/Admin/Courses/Create.cshtml",
            new CreateCourseViewModel()
        );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCourseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            PopulateSelectLists();
            return View("~/Views/Admin/Courses/Create.cshtml", model);
        }

        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description ?? string.Empty,
            Price = model.Price,
            Language = model.Language,
            Level = model.Level,
            Rating = model.Rating,
            Discount = model.Discount,
            InstructorId = model.InstructorId,
            TrackStackId = model.TrackStackId,
            Duration = new CourseDuration
            {
                Value = model.DurationValue,
                DurationIn = model.DurationIn
            }
        };

        if (model.Image != null && model.Image.Length > 0)
        {
            var uploads = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploads);

            var fileName = $"course_{Guid.NewGuid()}{Path.GetExtension(model.Image.FileName)}";
            var filePath = Path.Combine(uploads, fileName);

            using (var stream = System.IO.File.Create(filePath))
            {
                await model.Image.CopyToAsync(stream);
            }

            course.ImageUrl = $"/uploads/{fileName}";
        }

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var course = await _context.Courses
            .Include(c => c.Duration)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
            return NotFound();

        PopulateSelectLists();

        var model = new EditCourseViewModel
        {
            Id = course.Id,
            Name = course.Name,
            Description = course.Description,
            Price = course.Price,
            Language = course.Language,
            Level = course.Level,
            Rating = course.Rating,
            Discount = course.Discount,
            InstructorId = course.InstructorId,
            TrackStackId = course.TrackStackId,
            DurationValue = course.Duration?.Value ?? 0,
            DurationIn = course.Duration?.DurationIn ?? DurationIn.Days
        };

        return View("~/Views/Admin/Courses/Edit.cshtml", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EditCourseViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            PopulateSelectLists();
            return View("~/Views/Admin/Courses/Edit.cshtml", model);
        }

        var course = await _context.Courses
            .Include(c => c.Duration)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
            return NotFound();

        course.Name = model.Name;
        course.Description = model.Description ?? string.Empty;
        course.Price = model.Price;
        course.Language = model.Language;
        course.Level = model.Level;
        course.Rating = model.Rating;
        course.Discount = model.Discount;
        course.InstructorId = model.InstructorId;
        course.TrackStackId = model.TrackStackId;

        if (course.Duration == null)
        {
            course.Duration = new CourseDuration();
        }

        course.Duration.Value = model.DurationValue;
        course.Duration.DurationIn = model.DurationIn;

        if (model.Image != null && model.Image.Length > 0)
        {
            var uploads = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploads);

            var fileName = $"course_{Guid.NewGuid()}{Path.GetExtension(model.Image.FileName)}";
            var filePath = Path.Combine(uploads, fileName);

            using (var stream = System.IO.File.Create(filePath))
            {
                await model.Image.CopyToAsync(stream);
            }

            course.ImageUrl = "/uploads/" + fileName;
        }

        _context.Courses.Update(course);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var course = await _context.Courses
            .Include(c => c.Instructor)
                .ThenInclude(i => i.User)
            .Include(c => c.TrackStack)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
            return NotFound();


        var model = new CourseDetailsViewModel
        {
            Id = course.Id,
            Name = course.Name,
            Description = course.Description,
            Price = course.Price,
            ImageUrl = course.ImageUrl,
            Language = course.Language,
            Level = course.Level,
            Rating = course.Rating,
            Discount = course.Discount,

            DurationValue = course.Duration?.Value ?? 0,
            DurationIn = course.Duration?.DurationIn ?? DurationIn.Days,

            InstructorName = course.Instructor?.User != null
                ? course.Instructor.User.FirstName + " " + course.Instructor.User.LastName
                : "No Instructor",

            TrackStackName = course.TrackStack?.StackName ?? "No Track Stack"
        };


        return View("~/Views/Admin/Courses/Delete.cshtml", model);
    }
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var course = await _context.Courses.FindAsync(id);

        if (course == null)
            return NotFound();

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private void PopulateSelectLists()
    {
        ViewBag.Instructors = new SelectList(_context.Instructors.Include(i => i.User).Select(i => new { i.UserId, Name = i.User.FirstName + " " + i.User.LastName }), "UserId", "Name");
        ViewBag.TrackStacks = new SelectList(_context.TrackStacks.Select(ts => new { ts.Id, Name = ts.StackName }), "Id", "Name");
    }
}
