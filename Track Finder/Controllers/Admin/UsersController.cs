using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackFinder.Context;
using TrackFinder.Models.UserModels;
using TrackFinder.ViewModels.Users;
using TrackFinder.Models.CourseModels;
using TrackFinder.Models.CommunityModels;
using TrackFinder.Models.OrdersAndPaymentsModels;
using TrackFinder.Models.AssessmentModels;

namespace TrackFinder.Controllers.Admin;

[Authorize]
public class UsersController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public UsersController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // ========================= INDEX =========================

    public async Task<IActionResult> Index()
    {
        var users = await _context.Users
            .Select(u => new UserListViewModel
            {
                Id = u.Id,
                Username = u.UserName,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Role = u.Instructor != null ? UserRole.Instructor : (u.Student != null ? UserRole.Student : UserRole.Admin),
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();

        return View("~/Views/Admin/Users/Index.cshtml", users);
    }

    // ========================= DETAILS =========================

    public async Task<IActionResult> Details(Guid id)
    {
        var user = await _context.Users
            .Include(u => u.Instructor)
            .Include(u => u.Student)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
            return NotFound();

        var model = new UserDetailsViewModel
        {
            Id = user.Id,
            Username = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Birthdate = user.Birthdate,
            Gender = user.Gender,
            Bio = user.Bio,
            ProfilePictureUrl = user.ProfilePictureUrl,
            EmailVerified = user.EmailVerified,
            IsBanned = user.IsBanned,
            CreatedAt = user.CreatedAt,
            Role = user.Instructor != null ? UserRole.Instructor : (user.Student != null ? UserRole.Student : UserRole.Admin)
        };

        return View("~/Views/Admin/Users/Details.cshtml", model);
    }

    // ========================= CREATE =========================

    [HttpGet]
    public IActionResult Create()
    {
        return View("~/Views/Admin/Users/Create.cshtml",
            new CreateUserViewModel
            {
                Birthdate = DateTime.Today
            });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        if (!ModelState.IsValid)
            return View("~/Views/Admin/Users/Create.cshtml", model);

        if (await _context.Users.AnyAsync(x => x.Email == model.Email))
        {
            ModelState.AddModelError("Email", "Email already exists.");
            return View("~/Views/Admin/Users/Create.cshtml", model);
        }

        if (await _context.Users.AnyAsync(x => x.UserName == model.Username))
        {
            ModelState.AddModelError("Username", "Username already exists.");
            return View("~/Views/Admin/Users/Create.cshtml", model);
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = model.Username,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Birthdate = model.Birthdate,
            Gender = model.Gender,
            Bio = model.Bio,
            EmailVerified = model.EmailVerified,
            IsBanned = model.IsBanned,
            Role = model.Role,
            CreatedAt = DateTime.UtcNow
        };

        // Hash Password
        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

        if (model.Image != null && model.Image.Length > 0)
        {
            var uploads = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var fileName = Guid.NewGuid() + Path.GetExtension(model.Image.FileName);
            var filePath = Path.Combine(uploads, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await model.Image.CopyToAsync(stream);

            user.ProfilePictureUrl = "/uploads/" + fileName;
        }

        _context.Users.Add(user);

        // Add role-specific profile record
        if (model.Role == UserRole.Instructor)
        {
            _context.Instructors.Add(new Instructor
            {
                UserId = user.Id,
                AdminApproved = true
            });
        }
        else if (model.Role == UserRole.Student)
        {
            _context.Students.Add(new Student
            {
                UserId = user.Id
            });
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // ========================= EDIT =========================

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var user = await _context.Users
            .Include(u => u.Instructor)
            .Include(u => u.Student)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return NotFound();

        var model = new EditUserViewModel
        {
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Birthdate = user.Birthdate,
            Gender = user.Gender,
            Bio = user.Bio,
            EmailVerified = user.EmailVerified,
            IsBanned = user.IsBanned,
            Role = user.Instructor != null ? UserRole.Instructor : (user.Student != null ? UserRole.Student : UserRole.Admin)
        };

        return View("~/Views/Admin/Users/Edit.cshtml", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EditUserViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View("~/Views/Admin/Users/Edit.cshtml", model);

        var user = await _context.Users
            .Include(u => u.Instructor)
            .Include(u => u.Student)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return NotFound();

        if (await _context.Users.AnyAsync(x => x.Email == model.Email && x.Id != id))
        {
            ModelState.AddModelError("Email", "Email already exists.");
            return View("~/Views/Admin/Users/Edit.cshtml", model);
        }

        if (await _context.Users.AnyAsync(x => x.UserName == model.Username && x.Id != id))
        {
            ModelState.AddModelError("Username", "Username already exists.");
            return View("~/Views/Admin/Users/Edit.cshtml", model);
        }

        // Check if role changed
        var currentRole = user.Instructor != null ? UserRole.Instructor : (user.Student != null ? UserRole.Student : UserRole.Admin);
        if (currentRole != model.Role)
        {
            // Remove old role profile
            if (currentRole == UserRole.Instructor && user.Instructor != null)
            {
                _context.PurchasedItems.RemoveRange(_context.PurchasedItems.Where(p => p.InstructorId == id));
                _context.Communities.RemoveRange(_context.Communities.Where(c => c.AdminId == id));
                _context.Courses.RemoveRange(_context.Courses.Where(c => c.InstructorId == id));
                _context.Instructors.Remove(user.Instructor);
            }
            else if (currentRole == UserRole.Student && user.Student != null)
            {
                _context.StudentAnswers.RemoveRange(_context.StudentAnswers.Where(sa => sa.StudentId == id));
                _context.StudentCertificates.RemoveRange(_context.StudentCertificates.Where(sc => sc.StudentId == id));
                _context.AssessmentResults.RemoveRange(_context.AssessmentResults.Where(ar => ar.UserId == id));
                _context.JoinedMembers.RemoveRange(_context.JoinedMembers.Where(jm => jm.MemberId == id));
                _context.Enrollments.RemoveRange(_context.Enrollments.Where(e => e.UserId == id));
                _context.PurchasedItems.RemoveRange(_context.PurchasedItems.Where(p => p.StudentId == id));
                _context.Students.Remove(user.Student);
            }

            // Create new role profile
            if (model.Role == UserRole.Instructor)
            {
                _context.Instructors.Add(new Instructor
                {
                    UserId = user.Id,
                    AdminApproved = true
                });
            }
            else if (model.Role == UserRole.Student)
            {
                _context.Students.Add(new Student
                {
                    UserId = user.Id
                });
            }
        }

        user.UserName = model.Username;
        user.Email = model.Email;
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Birthdate = model.Birthdate;
        user.Gender = model.Gender;
        user.Bio = model.Bio;
        user.EmailVerified = model.EmailVerified;
        user.IsBanned = model.IsBanned;
        user.Role = model.Role;

        if (!string.IsNullOrWhiteSpace(model.Password))
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
        }

        if (model.Image != null && model.Image.Length > 0)
        {
            var uploads = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var fileName = Guid.NewGuid() + Path.GetExtension(model.Image.FileName);
            var filePath = Path.Combine(uploads, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await model.Image.CopyToAsync(stream);

            user.ProfilePictureUrl = "/uploads/" + fileName;
        }

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // ========================= DELETE =========================

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var user = await _context.Users
            .Include(u => u.Instructor)
            .Include(u => u.Student)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
            return NotFound();

        var model = new UserDetailsViewModel
        {
            Id = user.Id,
            Username = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Birthdate = user.Birthdate,
            Gender = user.Gender,
            Bio = user.Bio,
            ProfilePictureUrl = user.ProfilePictureUrl,
            EmailVerified = user.EmailVerified,
            IsBanned = user.IsBanned,
            CreatedAt = user.CreatedAt,
            Role = user.Instructor != null ? UserRole.Instructor : (user.Student != null ? UserRole.Student : UserRole.Admin)
        };

        return View("~/Views/Admin/Users/Delete.cshtml", model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var user = await _context.Users
            .Include(u => u.Instructor)
            .Include(u => u.Student)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return NotFound();

        // 1. Clean up community comments/posts/reports made by this user
        _context.PostReports.RemoveRange(_context.PostReports.Where(pr => pr.ReporterId == id));
        _context.Comments.RemoveRange(_context.Comments.Where(c => c.UserId == id));
        _context.Posts.RemoveRange(_context.Posts.Where(p => p.UserId == id));

        // 2. Clean up Student-specific records
        if (user.Student != null)
        {
            _context.StudentAnswers.RemoveRange(_context.StudentAnswers.Where(sa => sa.StudentId == id));
            _context.StudentCertificates.RemoveRange(_context.StudentCertificates.Where(sc => sc.StudentId == id));
            _context.AssessmentResults.RemoveRange(_context.AssessmentResults.Where(ar => ar.UserId == id));
            _context.JoinedMembers.RemoveRange(_context.JoinedMembers.Where(jm => jm.MemberId == id));
            _context.Enrollments.RemoveRange(_context.Enrollments.Where(e => e.UserId == id));
            _context.PurchasedItems.RemoveRange(_context.PurchasedItems.Where(p => p.StudentId == id));
            _context.Students.Remove(user.Student);
        }

        // 3. Clean up Instructor-specific records
        if (user.Instructor != null)
        {
            _context.PurchasedItems.RemoveRange(_context.PurchasedItems.Where(p => p.InstructorId == id));
            _context.Communities.RemoveRange(_context.Communities.Where(c => c.AdminId == id));
            _context.Courses.RemoveRange(_context.Courses.Where(c => c.InstructorId == id));
            _context.Instructors.Remove(user.Instructor);
        }

        // 4. Finally, remove the User
        _context.Users.Remove(user);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}