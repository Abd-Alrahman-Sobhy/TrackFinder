using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackFinder.Models.AssessmentModels;
using TrackFinder.ViewModels.Tracks;
using TrackFinder.Context;

namespace TrackFinder.Controllers.Admin;

[Authorize]
public class TracksController : Controller
{
    private readonly AppDbContext _context;

    public TracksController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var tracks = await _context.Tracks.ToListAsync();
        var vm = tracks.Select(t => new TrackListViewModel
        {
            Id = t.Id,
            TrackName = t.TrackName,
            TrackDescription = t.TrackDescription
        }).ToList();
        return View("~/Views/Admin/Tracks/Index.cshtml", vm);
    }

    public async Task<IActionResult> Details(int id)
    {
        var track = await _context.Tracks.FirstOrDefaultAsync(t => t.Id == id);
        if (track == null) return NotFound();
        var vm = new TrackDetailsViewModel
        {
            Id = track.Id,
            TrackName = track.TrackName,
            TrackDescription = track.TrackDescription,
            RoadMapUrl = track.RoadMapUrl
        };
        return View("~/Views/Admin/Tracks/Details.cshtml", vm);
    }

    public IActionResult Create()
    {
        return View("~/Views/Admin/Tracks/Create.cshtml", new CreateTrackViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTrackViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Admin/Tracks/Create.cshtml", model);
        }

        var track = new Track { TrackName = model.TrackName, TrackDescription = model.TrackDescription ?? string.Empty, RoadMapUrl = model.RoadMapUrl };
        _context.Tracks.Add(track);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var track = await _context.Tracks.FindAsync(id);
        if (track == null) return NotFound();
        var vm = new EditTrackViewModel
        {
            Id = track.Id,
            TrackName = track.TrackName,
            TrackDescription = track.TrackDescription,
            RoadMapUrl = track.RoadMapUrl
        };
        return View("~/Views/Admin/Tracks/Edit.cshtml", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditTrackViewModel model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            return View("~/Views/Admin/Tracks/Edit.cshtml", model);
        }

        var track = await _context.Tracks.FindAsync(id);
        if (track == null) return NotFound();
        track.TrackName = model.TrackName;
        track.TrackDescription = model.TrackDescription ?? string.Empty;
        track.RoadMapUrl = model.RoadMapUrl;
        _context.Tracks.Update(track);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var track = await _context.Tracks.FindAsync(id);
        if (track == null) return NotFound();
        var vm = new TrackDetailsViewModel
        {
            Id = track.Id,
            TrackName = track.TrackName,
            TrackDescription = track.TrackDescription,
            RoadMapUrl = track.RoadMapUrl
        };
        return View("~/Views/Admin/Tracks/Delete.cshtml", vm);
    }

    [HttpPost, ActionName("DeleteConfirmed")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var track = await _context.Tracks.FindAsync(id);
        if (track == null) return NotFound();
        _context.Tracks.Remove(track);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
