using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackFinder.Context;
using TrackFinder.Models.CourseModels;
using TrackFinder.ViewModels;

namespace TrackFinder.Controllers;

public class MaterialController : Controller
{
	private readonly AppDbContext _context;
	private readonly IWebHostEnvironment _webHostEnvironment;

	public MaterialController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
	{
		_context = context;
		_webHostEnvironment = webHostEnvironment;
	}

	public async Task<IActionResult> Details(Guid id)
	{
		var material = await _context.Materials
			.FindAsync(id);

		if (material is null)
			return NotFound();

		return View(material);
	}

	[HttpGet]
	public async Task<IActionResult> Create(Guid id)
	{
		var lesson = await _context.Lessons
			.Select(l => new { l.Id, l.CourseId })
			.FirstOrDefaultAsync(l => l.Id == id);

		if (lesson == null)
			return NotFound();

		var model = new MaterialUploadVM { LessonId = id };

		ViewBag.CourseId = lesson.CourseId;

		return View(model);
	}

	[ValidateAntiForgeryToken]
	[HttpPost]
	public async Task<IActionResult> Create(MaterialUploadVM model)
	{
		var lesson = await _context.Lessons
			.Select(l => new { l.Id, l.CourseId })
			.FirstOrDefaultAsync(l => l.Id == model.LessonId);

		if (lesson == null)
			return NotFound();

		ViewBag.CourseId = lesson.CourseId;

		if (!ModelState.IsValid)
			return View(model);

		if (model.File == null || model.File.Length == 0)
		{
			ModelState.AddModelError("File", "Please select a file to upload.");
			return View(model);
		}

		var extension = Path.GetExtension(model.File.FileName).ToLower();
		var allowedExtensions = new[] { ".rar", ".zip", ".pdf", ".doc", ".docx", ".ppt", ".pptx", ".xls", ".xlsx", ".txt", ".png", ".jpg", ".jpeg", ".mp4", ".avi", ".mkv" };
		if (!allowedExtensions.Contains(extension))
		{
			ModelState.AddModelError("File", "File type not supported. Allowed: " + string.Join(", ", allowedExtensions));
			return View(model);
		}

		string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "materials");
		if (!Directory.Exists(uploadsFolder))
		{
			Directory.CreateDirectory(uploadsFolder);
		}

		string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.File.FileName;
		string filePath = Path.Combine(uploadsFolder, uniqueFileName);

		using (var fileStream = new FileStream(filePath, FileMode.Create))
		{
			await model.File.CopyToAsync(fileStream);
		}

		var material = new Material
		{
			Id = Guid.NewGuid(),
			FileName = model.File.FileName,
			FileUrl = "/materials/" + uniqueFileName,
			ContentType = model.File.ContentType,
			LessonId = model.LessonId,
			CourseId = lesson.CourseId ?? Guid.Empty
		};

		_context.Materials.Add(material);
		await _context.SaveChangesAsync();

		return RedirectToAction(nameof(Details), new { id = material.Id });
	}

	[HttpGet]
	public async Task<IActionResult> Download(Guid id)
	{
		var material = await _context.Materials.FindAsync(id);

		if (material == null)
			return NotFound();

		return View(material);
	}

	[HttpPost, ActionName("Download")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DownloadConfirmed(Guid id)
	{
		var material = await _context.Materials.FindAsync(id);

		if (material == null)
			return NotFound();

		var filePath = Path.Combine(_webHostEnvironment.WebRootPath, material.FileUrl.TrimStart('/'));

		if (!System.IO.File.Exists(filePath))
			return NotFound("File not found on the server.");

		byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
		return File(fileBytes, material.ContentType, material.FileName);
	}
}
