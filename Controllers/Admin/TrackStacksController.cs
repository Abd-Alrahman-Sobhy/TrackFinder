using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrackFinder.Context;
using TrackFinder.Models.AssessmentModels;
using TrackFinder.ViewModels.TrackStacks;

namespace TrackFinder.Controllers.Admin
{
    [Authorize]
    public class TrackStacksController : Controller
    {
        private readonly AppDbContext _context;

        public TrackStacksController(AppDbContext context)
        {
            _context = context;
        }

        private void LoadTracks(int? selected = null)
        {
            ViewBag.Tracks = new SelectList(
                _context.Tracks.Select(t => new
                {
                    t.Id,
                    t.TrackName
                }),
                "Id",
                "Name",
                selected
            );
        }

        //================== INDEX ==================

        public async Task<IActionResult> Index()
        {
            var model = await _context.TrackStacks
                .Include(ts => ts.RelatedTrack)
                .Include(ts => ts.Courses)
                .Select(ts => new TrackStackListViewModel
                {
                    Id = ts.Id,
                    StackName = ts.StackName,
                    StackDescription = ts.StackDescription,
                    RelatedTrackName = ts.RelatedTrack.TrackName,
                    CoursesCount = ts.Courses.Count()
                })
                .ToListAsync();

            return View("~/Views/Admin/TrackStacks/Index.cshtml", model);
        }

        //================== DETAILS ==================

        public async Task<IActionResult> Details(int id)
        {
            var stack = await _context.TrackStacks
                .Include(ts => ts.RelatedTrack)
                .Include(ts => ts.Courses)
                .FirstOrDefaultAsync(ts => ts.Id == id);

            if (stack == null)
                return NotFound();

            var model = new TrackStackDetailsViewModel
            {
                Id = stack.Id,
                StackName = stack.StackName,
                StackDescription = stack.StackDescription,
                RelatedTrackName = stack.RelatedTrack.TrackName,
                CoursesCount = stack.Courses.Count()
            };

            return View("~/Views/Admin/TrackStacks/Details.cshtml", model);
        }

        //================== CREATE ==================

       
        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateTrackStackViewModel();

            model.Tracks = _context.Tracks
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.TrackName
                })
                .ToList();

            return View("~/Views/Admin/TrackStacks/Create.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTrackStackViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Tracks = _context.Tracks
                    .Select(t => new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.TrackName,
                        Selected = t.Id == model.RelatedTrackId
                    })
                    .ToList();

                return View("~/Views/Admin/TrackStacks/Create.cshtml", model);
            }


            var stack = new TrackStack
            {
                StackName = model.StackName,
                StackDescription = model.StackDescription ?? "",
                RelatedTrackId = model.RelatedTrackId
            };


            _context.TrackStacks.Add(stack);

            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }

        //================== EDIT ==================

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var stack = await _context.TrackStacks
                .FirstOrDefaultAsync(x => x.Id == id);

            if (stack == null)
                return NotFound();


            var model = new EditTrackStackViewModel
            {
                Id = stack.Id,
                StackName = stack.StackName,
                StackDescription = stack.StackDescription,
                RelatedTrackId = stack.RelatedTrackId
            };


            ViewBag.Tracks = new SelectList(
                _context.Tracks,
                "Id",
                "TrackName",
                stack.RelatedTrackId
            );


            return View("~/Views/Admin/TrackStacks/Edit.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditTrackStackViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                LoadTracks(model.RelatedTrackId);
                return View("~/Views/Admin/TrackStacks/Edit.cshtml", model);
            }

            var stack = await _context.TrackStacks.FindAsync(id);

            if (stack == null)
                return NotFound();

            stack.StackName = model.StackName;
            stack.StackDescription = model.StackDescription ?? "";
            stack.RelatedTrackId = model.RelatedTrackId;

            _context.TrackStacks.Update(stack);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //================== DELETE ==================

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var stack = await _context.TrackStacks
                .Include(ts => ts.RelatedTrack)
                .Include(ts => ts.Courses)
                .FirstOrDefaultAsync(ts => ts.Id == id);

            if (stack == null)
                return NotFound();

            var model = new TrackStackDetailsViewModel
            {
                Id = stack.Id,
                StackName = stack.StackName,
                StackDescription = stack.StackDescription,
                RelatedTrackName = stack.RelatedTrack.TrackName,
                CoursesCount = stack.Courses.Count()
            };

            return View("~/Views/Admin/TrackStacks/Delete.cshtml", model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stack = await _context.TrackStacks.FindAsync(id);

            if (stack == null)
                return NotFound();

            _context.TrackStacks.Remove(stack);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}