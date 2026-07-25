using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackFinder.Models.AchievementModels;
using TrackFinder.ViewModels.Achievements;
using TrackFinder.Context;

namespace TrackFinder.Controllers.Admin;

[Authorize]
public class CertificatesController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;


    public CertificatesController(
        AppDbContext context,
        IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }



   

    public async Task<IActionResult> Index()
    {
        var certificates = await _context.Certificates
            .Select(c => new CertificateListViewModel
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                CertificateUrl = c.CertificateUrl
            })
            .ToListAsync();


        return View("~/Views/Admin/Certificates/Index.cshtml", certificates);
    }




    
    public async Task<IActionResult> Details(int id)
    {
        var certificate = await _context.Certificates
            .FirstOrDefaultAsync(c => c.Id == id);


        if (certificate == null)
            return NotFound();



        var model = MapToDetailsViewModel(certificate);


        return View("~/Views/Admin/Certificates/Details.cshtml", model);
    }





    // ================= CREATE GET =================

    [HttpGet]
    public IActionResult Create()
    {
        return View(
            "~/Views/Admin/Certificates/Create.cshtml",
            new CreateCertificateViewModel()
        );
    }





  

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateCertificateViewModel model)
    {

        if (!ModelState.IsValid)
        {
            return View(
                "~/Views/Admin/Certificates/Create.cshtml",
                model);
        }



        var certificate = new Certificate
        {
            Title = model.Title,
            Description = model.Description
        };



        if (model.File != null && model.File.Length > 0)
        {
            certificate.CertificateUrl =
                await UploadFile(model.File, "certificate");
        }



        _context.Certificates.Add(certificate);

        await _context.SaveChangesAsync();


        return RedirectToAction(nameof(Index));
    }






   

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var certificate = await _context.Certificates
            .FirstOrDefaultAsync(c => c.Id == id);



        if (certificate == null)
            return NotFound();



        var model = new EditCertificateViewModel
        {
            Id = certificate.Id,
            Title = certificate.Title,
            Description = certificate.Description
        };


        return View(
            "~/Views/Admin/Certificates/Edit.cshtml",
            model);
    }





   

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        EditCertificateViewModel model)
    {

        if (id != model.Id)
            return BadRequest();



        if (!ModelState.IsValid)
        {
            return View(
                "~/Views/Admin/Certificates/Edit.cshtml",
                model);
        }



        var certificate = await _context.Certificates
            .FirstOrDefaultAsync(c => c.Id == id);



        if (certificate == null)
            return NotFound();



        certificate.Title = model.Title;
        certificate.Description = model.Description;



        if (model.File != null && model.File.Length > 0)
        {
            certificate.CertificateUrl =
                await UploadFile(model.File, "certificate");
        }



        _context.Certificates.Update(certificate);

        await _context.SaveChangesAsync();



        return RedirectToAction(nameof(Index));
    }






    // ================= DELETE GET =================

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Certificates.FindAsync(id);

        if (item == null)
            return NotFound();

        var vm = new CertificateDetailsViewModel
        {
            Id = item.Id,
            Title = item.Title,
            Description = item.Description,
            CertificateUrl = item.CertificateUrl
        };

        return View("~/Views/Admin/Certificates/Delete.cshtml", vm);
    }





    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _context.Certificates.FindAsync(id);

        if (item == null)
            return NotFound();

        _context.Certificates.Remove(item);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }





   


    private CertificateDetailsViewModel MapToDetailsViewModel(
        Certificate certificate)
    {
        return new CertificateDetailsViewModel
        {
            Id = certificate.Id,
            Title = certificate.Title,
            Description = certificate.Description,
            CertificateUrl = certificate.CertificateUrl
        };
    }




    private async Task<string> UploadFile(
        IFormFile file,
        string prefix)
    {

        var uploads =
            Path.Combine(_env.WebRootPath, "uploads");


        Directory.CreateDirectory(uploads);



        var fileName =
            $"{prefix}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";



        var filePath =
            Path.Combine(uploads, fileName);



        using (var stream = System.IO.File.Create(filePath))
        {
            await file.CopyToAsync(stream);
        }



        return $"/uploads/{fileName}";
    }

}