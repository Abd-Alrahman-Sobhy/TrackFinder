using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackFinder.Context;
using TrackFinder.Models.CourseModels;

namespace TrackFinder.Controllers;

public class LessonController : Controller
{
	private readonly AppDbContext _context;

	public LessonController(AppDbContext context)
	{
		_context = context;
	}

	public async Task<IActionResult> Details(Guid id)
	{
		var lesson = await _context.Lessons
			.Include(l => l.Materials)
			.AsNoTracking()
			.FirstOrDefaultAsync(l => l.Id == id);

		if (lesson is null)
			return NotFound();

		return View(lesson);
	}

	[HttpGet]
	public async Task<IActionResult> Create(Guid id)
	{
		var courseExists = await _context.Courses
			.AnyAsync(c => c.Id == id);

		if (!courseExists)
			return NotFound();

		var lesson = new Lesson { CourseId = id };

		return View(lesson);
	}

	[ValidateAntiForgeryToken]
	[HttpPost]
	public async Task<IActionResult> Create(Lesson lesson)
	{
		// Force a new Guid to prevent ASP.NET Core from binding the route '{id}' (Course ID) to 'lesson.Id'
		lesson.Id = Guid.NewGuid();

		if (!ModelState.IsValid)
			return View(lesson);

		var courseExists = await _context.Courses
			.AnyAsync(c => c.Id == lesson.CourseId);

		if (!courseExists)
			return NotFound();

		await _context.Lessons.AddAsync(lesson);
		await _context.SaveChangesAsync();

		return RedirectToAction("Details", "CourseCreation", new { id = lesson.CourseId });
	}

	[HttpGet]
	public async Task<IActionResult> Edit(Guid id)
	{
		var lesson = await _context.Lessons
			.FindAsync(id);

		if (lesson is null)
			return NotFound();

		return View(lesson);
	}

	[ValidateAntiForgeryToken]
	[HttpPost]
	public async Task<IActionResult> Edit(Guid id, Lesson request)
	{
		if (!ModelState.IsValid)
			return View(request);

		var lesson = await _context.Lessons
			.FindAsync(id);

		if (lesson is null)
			return NotFound();

		lesson.MapFrom(request);

		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Details), new { id });
	}

	public async Task<IActionResult> Delete(Guid id)
	{
		var lesson = await _context.Lessons
			.Include(l => l.Materials)
			.FirstOrDefaultAsync(l => l.Id == id);

		if (lesson is null)
			return NotFound();

		_context.Materials.RemoveRange(lesson.Materials);
		_context.Lessons.Remove(lesson);
		await _context.SaveChangesAsync();

		return RedirectToAction("Details", "CourseCreation", new { id = lesson.CourseId });
	}
}
