using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackFinder.Context;
using TrackFinder.Models.CourseModels;
using TrackFinderDb.Models.TeachingModels;

namespace TrackFinderDb.Controllers
{
    public class CourseSalesController : Controller
    {
        private readonly AppDbContext _context;

        public CourseSalesController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var courses = _context.Courses.ToList();

            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (Guid.TryParse(userIdString, out var userId))
                {
                    var enrolledIds = _context.Enrollments
                        .Where(e => e.UserId == userId)
                        .Select(e => e.CourseId)
                        .ToList();
                    ViewBag.EnrolledCourseIds = enrolledIds;
                }
            }

            return View(courses);
        }

        public IActionResult Details(Guid id)
        {
            var item = _context.Courses
                .Include(c => c.Lessons)
                .ThenInclude(l => l.Materials)
                .FirstOrDefault(x => x.Id == id);
            if (item == null)
                return NotFound();

            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (Guid.TryParse(userIdString, out var userId))
                {
                    ViewBag.IsEnrolled = _context.Enrollments
                        .Any(e => e.CourseId == id && e.UserId == userId);
                }
            }

            return View(item);
        }

        [Authorize]
        public IActionResult MyCourses()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return RedirectToAction("Index", "Login");

            var myCourses = _context.Courses
                .Where(c => _context.Enrollments.Any(e =>
                    e.CourseId == c.Id && e.UserId == userId))
                .ToList();

            return View(myCourses);
        }

        public IActionResult EnrollCourse(Guid courseId)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null)
                return NotFound();

            var enrollment = new Enrollment
            {
                CourseId = courseId
            };

            return View("EnrollmentForm", enrollment);
        }

        [HttpPost]
        [Authorize]
        public IActionResult EnrollCourse(Enrollment model)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return RedirectToAction("Index", "Login");

            model.Course = null;
            model.User = null;

            var already = _context.Enrollments.Any(e =>
                e.CourseId == model.CourseId &&
                e.UserId == userId);

            if (already)
            {
                TempData["Error"] = "You are already enrolled in this course.";
                return RedirectToAction("Index");
            }

            model.UserId = userId;
            model.EnrollmentDate = DateTime.Now;
            model.Status = EnrollmentStatus.NotStarted;
            model.Progress = 0;
            model.CompletedLessons = 0;
            _context.Entry(model).Property("StudentUserId").CurrentValue = userId;

            _context.Enrollments.Add(model);
            _context.SaveChanges();

            TempData["Success"] = "Enrollment completed successfully.";
            return RedirectToAction("MyCourses");
        }
    }
}
