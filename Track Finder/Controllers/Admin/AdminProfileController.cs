using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackFinder.Context;
using TrackFinder.ViewModels.Users;

namespace TrackFinder.Controllers.Admin
{
    [Authorize]
    public class AdminProfileController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AdminProfileController(AppDbContext context,
                                      IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

       

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (id == null)
                return RedirectToAction("Login", "Account");

            var admin = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == Guid.Parse(id));

            if (admin == null)
                return NotFound();

            var model = new UserDetailsViewModel
            {
                Id = admin.Id,
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                Username = admin.UserName,
                Email = admin.Email,
                Birthdate = admin.Birthdate,
                Gender = admin.Gender,
                Bio = admin.Bio,
                ProfilePictureUrl = admin.ProfilePictureUrl,
                EmailVerified = admin.EmailVerified,
                CreatedAt = admin.CreatedAt,
                Role = admin.Role,
                IsBanned = admin.IsBanned
            };

            return View("~/Views/Admin/Profile/Index.cshtml", model);
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(UserDetailsViewModel model, IFormFile? Image)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Admin/Profile/Index.cshtml", model);

            var admin = await _context.Users.FindAsync(model.Id);

            if (admin == null)
                return NotFound();

            admin.FirstName = model.FirstName;
            admin.LastName = model.LastName;
            admin.UserName = model.Username;
            admin.Email = model.Email;
            admin.Birthdate = model.Birthdate;
            admin.Gender = model.Gender;
            admin.Bio = model.Bio;

            if (Image != null && Image.Length > 0)
            {
                var uploads = Path.Combine(_environment.WebRootPath, "uploads");

                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid() +
                               Path.GetExtension(Image.FileName);

                var path = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }

                admin.ProfilePictureUrl = "/uploads/" + fileName;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Profile updated successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}