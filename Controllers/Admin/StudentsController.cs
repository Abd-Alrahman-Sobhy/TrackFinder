using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrackFinder.Context;
using TrackFinder.Models.UserModels;
using TrackFinder.ViewModels.Students;

namespace TrackFinder.Controllers.Admin;

[Authorize]
public class StudentsController : Controller
{
    private readonly AppDbContext _context;

    public StudentsController(AppDbContext context)
    {
        _context = context;
    }

    private void LoadUsers(Guid? selected = null)
    {
        ViewBag.Users = new SelectList(
            _context.Users.Select(u => new
            {
                u.Id,
                Name = u.FirstName + " " + u.LastName
            }),
            "Id",
            "Name",
            selected
        );
    }

    //================== INDEX ==================

    public async Task<IActionResult> Index()
    {
        var students = await _context.Students
    .Include(s => s.User)
    .Select(s => new StudentListViewModel
    {
        UserId = s.UserId,
        StudentName = s.User.FirstName + " " + s.User.LastName,
        Email = s.User.Email,
        ProfilePictureUrl = s.User.ProfilePictureUrl,
        SchoolOrUnversityName = s.SchoolOrUnversityName,
        Major = s.Major,
        AcademicYear = s.AcademicYear,
        GPA = s.GPA
    })
    .ToListAsync();

        return View("~/Views/Admin/Students/Index.cshtml", students);
    }

    //================== DETAILS ==================

    public async Task<IActionResult> Details(Guid userId)
    {
        var student = await _context.Students
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (student == null)
            return NotFound();

        var model = new StudentDetailsViewModel
        {
            UserId = student.UserId,
            StudentName = student.User.FirstName + " " + student.User.LastName,
            EducationState = student.EducationState,
            SchoolOrUnversityName = student.SchoolOrUnversityName,
            Major = student.Major,
            Minor = student.Minor,
            DegreeProgram = student.DegreeProgram,
            AcademicYear = student.AcademicYear,
            GPA = student.GPA,
            Bio = student.Bio
        };

        return View("~/Views/Admin/Students/Details.cshtml", model);
    }

    //================== CREATE ==================

    [HttpGet]
    public IActionResult Create()
    {
        LoadUsers();
        return View("~/Views/Admin/Students/Create.cshtml", new CreateStudentViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateStudentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            LoadUsers(model.UserId);
            return View("~/Views/Admin/Students/Create.cshtml", model);
        }

        var student = new Student
        {
            UserId = model.UserId,
            EducationState = model.EducationState,
            SchoolOrUnversityName = model.SchoolOrUnversityName,
            Major = model.Major,
            Minor = model.Minor,
            DegreeProgram = model.DegreeProgram,
            AcademicYear = model.AcademicYear,
            GPA = model.GPA,
            Bio = model.Bio
        };

        _context.Students.Add(student);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    //================== EDIT ==================

    [HttpGet]
    public async Task<IActionResult> Edit(Guid userId)
    {
        var student = await _context.Students.FindAsync(userId);

        if (student == null)
            return NotFound();

        LoadUsers(student.UserId);

        var model = new EditStudentViewModel
        {
            UserId = student.UserId,
            EducationState = student.EducationState,
            SchoolOrUnversityName = student.SchoolOrUnversityName,
            Major = student.Major,
            Minor = student.Minor,
            DegreeProgram = student.DegreeProgram,
            AcademicYear = student.AcademicYear,
            GPA = student.GPA,
            Bio = student.Bio
        };

        return View("~/Views/Admin/Students/Edit.cshtml", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid userId, EditStudentViewModel model)
    {
        if (userId != model.UserId)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            LoadUsers(model.UserId);
            return View("~/Views/Admin/Students/Edit.cshtml", model);
        }

        var student = await _context.Students.FindAsync(userId);

        if (student == null)
            return NotFound();

        student.EducationState = model.EducationState;
        student.SchoolOrUnversityName = model.SchoolOrUnversityName;
        student.Major = model.Major;
        student.Minor = model.Minor;
        student.DegreeProgram = model.DegreeProgram;
        student.AcademicYear = model.AcademicYear;
        student.GPA = model.GPA;
        student.Bio = model.Bio;

        _context.Update(student);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    //================== DELETE ==================

    [HttpGet]
    public async Task<IActionResult> Delete(Guid userId)
    {
        var student = await _context.Students
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (student == null)
            return NotFound();

        var model = new StudentDetailsViewModel
        {
            UserId = student.UserId,
            StudentName = student.User.FirstName + " " + student.User.LastName,
            EducationState = student.EducationState,
            SchoolOrUnversityName = student.SchoolOrUnversityName,
            Major = student.Major,
            Minor = student.Minor,
            DegreeProgram = student.DegreeProgram,
            AcademicYear = student.AcademicYear,
            GPA = student.GPA,
            Bio = student.Bio
        };

        return View("~/Views/Admin/Students/Delete.cshtml", model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid userId)
    {
        var student = await _context.Students.FindAsync(userId);

        if (student == null)
            return NotFound();

        _context.Students.Remove(student);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
