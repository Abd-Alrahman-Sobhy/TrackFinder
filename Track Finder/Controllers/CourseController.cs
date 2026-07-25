using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackFinder.Context;
using TrackFinder.Models.CourseModels;

namespace TrackFinder.Controllers;

[Authorize]
public class CourseController : Controller
{
    private readonly AppDbContext _context;

    public CourseController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(Guid id)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            return RedirectToAction("Index", "Login");

        var enrolled = await _context.Enrollments
            .AnyAsync(e => e.CourseId == id && e.UserId == userId);
        if (!enrolled)
            return RedirectToAction("Index", "CourseSales");

        var course = await _context.Courses
            .Include(c => c.Lessons)
                .ThenInclude(l => l.Materials)
            .Include(c => c.Lessons)
                .ThenInclude(l => l.Exams)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
            return NotFound();

        var enrollment = await _context.Enrollments
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.CourseId == id && e.UserId == userId);

        ViewBag.Enrollment = enrollment;

        return View("~/Views/Course/Index.cshtml", course);
    }

    public async Task<IActionResult> Lesson(Guid courseId, Guid lessonId)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            return RedirectToAction("Index", "Login");

        var enrolled = await _context.Enrollments
            .AnyAsync(e => e.CourseId == courseId && e.UserId == userId);
        if (!enrolled)
            return RedirectToAction("Index", "CourseSales");

        var lesson = await _context.Lessons
            .Include(l => l.Materials)
            .Include(l => l.Exams)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == lessonId && l.CourseId == courseId);

        if (lesson == null)
            return NotFound();

        return View("~/Views/Course/Lesson.cshtml", lesson);
    }
}
