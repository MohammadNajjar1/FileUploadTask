using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspNetCore.Objects.Models;
using AspNetCore.Data;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace AspNetCore.Controllers
{
    public class FileController : Controller
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _environment;

        public FileController(Context context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult UploadFile() => View();

        [HttpPost]
        public async Task<IActionResult> UploadFile(FileUploadModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.File.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(fileStream);
                }

                UploadedFile? uploadedFile = new UploadedFile
                {
                    Id = Guid.NewGuid(),
                    OriginalFileName = model.File.FileName,
                    SavedFileName = uniqueFileName,
                    Description = model.Description,
                    UploadDate = DateTime.Now
                };

                _context.Files.Add(uploadedFile);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "File uploaded successfully!";
                return RedirectToAction(nameof(DisplayFiles));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unexpected error occurred. Please try again.");
                return View(model);
            }
        }

        // GET: Display Files
        public IActionResult DisplayFiles()
        {
            List<UploadedFile> files = _context.Files.ToList();
            return View(files);
        }

        // GET: Download File
        public async Task<IActionResult> DownloadFile(Guid id)
        {
            UploadedFile? file = await _context.Files.FindAsync(id);
            if (file == null)
                return NotFound();

            string filePath = Path.Combine(_environment.WebRootPath, "uploads", file.SavedFileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            MemoryStream memory = new MemoryStream();
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, "application/octet-stream", file.OriginalFileName);
        }
    }
}
