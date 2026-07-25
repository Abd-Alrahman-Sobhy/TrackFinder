using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TrackFinder.Context;
using TrackFinder.Models.CourseModels;

namespace TrackFinder.Controllers;

/// <summary>
/// Handles course creation, editing, and management for the authenticated instructor.
/// Every action that reads or modifies course data is scoped to the current instructor's
/// UserId (stored in ClaimTypes.NameIdentifier by the login service).
/// </summary>
[Authorize(Roles = "Instructor")]
public class CourseCreationController : Controller
{
    private readonly AppDbContext _context;

    public CourseCreationController(AppDbContext context)
    {
        _context = context;
    }

    // ── Helper: get the logged-in instructor's Guid ────────────────────────
    private Guid GetCurrentInstructorId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(raw, out var id) ? id : Guid.Empty;
    }

    // ── GET /CourseCreation ────────────────────────────────────────────────
    /// <summary>Lists only courses owned by the logged-in instructor.</summary>
    public async Task<IActionResult> Index()
    {
        var instructorId = GetCurrentInstructorId();

        var courses = await _context.Courses
            .Where(c => c.InstructorId == instructorId)
            .AsNoTracking()
            .ToListAsync();

        return View(courses);
    }

    // ── GET /CourseCreation/Create ─────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.GainedSkills = await _context.GainedSkills
            .AsNoTracking()
            .ToListAsync();

        return View();
    }

    // ── POST /CourseCreation/Create ────────────────────────────────────────
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Create(Course request, List<int> selectedSkillIds)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.GainedSkills = await _context.GainedSkills
                .AsNoTracking()
                .ToListAsync();
            return View(request);
        }

        // Tie the new course to the logged-in instructor
        request.InstructorId = GetCurrentInstructorId();

        request.CourseSkills = (selectedSkillIds ?? [])
            .Distinct()
            .Select(skillId => new CourseSkill { GainedSkillId = skillId })
            .ToList();

        await _context.Courses.AddAsync(request);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // ── GET /CourseCreation/Details/{id} ───────────────────────────────────
    public async Task<IActionResult> Details(Guid id)
    {
        var instructorId = GetCurrentInstructorId();

        var course = await _context.Courses
            .Include(c => c.Lessons)
            .Include(c => c.CourseSkills)!
                .ThenInclude(cs => cs.GainedSkill)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.InstructorId == instructorId);

        if (course is null)
            return NotFound();

        return View(course);
    }

    // ── GET /CourseCreation/Edit/{id} ──────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var instructorId = GetCurrentInstructorId();

        var course = await _context.Courses
            .Include(c => c.CourseSkills)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.InstructorId == instructorId);

        if (course is null)
            return NotFound();

        ViewBag.GainedSkills = await _context.GainedSkills
            .AsNoTracking()
            .ToListAsync();

        ViewBag.SelectedSkillIds = course.CourseSkills
            .Select(cs => cs.GainedSkillId)
            .ToList();

        return View(course);
    }

    // ── POST /CourseCreation/Edit/{id} ─────────────────────────────────────
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, Course request, List<int> selectedSkillIds)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.GainedSkills = await _context.GainedSkills
                .AsNoTracking()
                .ToListAsync();

            ViewBag.SelectedSkillIds = selectedSkillIds;

            return View(request);
        }

        var instructorId = GetCurrentInstructorId();

        var course = await _context.Courses
            .Include(c => c.CourseSkills)
            .FirstOrDefaultAsync(c => c.Id == id && c.InstructorId == instructorId);

        if (course is null)
            return NotFound();

        course.MapFrom(request);

        var distinctSkillIds = (selectedSkillIds ?? []).Distinct().ToHashSet();

        var skillsToRemove = course.CourseSkills!
            .Where(cs => !distinctSkillIds.Contains(cs.GainedSkillId!))
            .ToList();

        foreach (var skill in skillsToRemove)
            course.CourseSkills.Remove(skill);

        var existingSkillIds = course.CourseSkills
            .Select(cs => cs.GainedSkillId)
            .ToHashSet();

        foreach (var skillId in distinctSkillIds.Where(sId => !existingSkillIds.Contains(sId)))
            course.CourseSkills.Add(new CourseSkill { CourseId = course.Id, GainedSkillId = skillId });

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id });
    }

    // ── GET /CourseCreation/Delete/{id} ────────────────────────────────────
    public async Task<IActionResult> Delete(Guid id)
    {
        var instructorId = GetCurrentInstructorId();

        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == id && c.InstructorId == instructorId);

        if (course is null)
            return NotFound();

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
