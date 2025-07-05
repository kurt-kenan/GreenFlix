using GreenFlix.Data;
using GreenFlix.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GreenFlix.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var movies = _context.Movies.ToList();
            return View(movies);
        }

        [HttpGet]
        public IActionResult FilmEkle()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FilmEkle(Movie model, IFormFile PosterFile)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    foreach (var err in error.Value.Errors)
                    {
                        Console.WriteLine($"HATA ({error.Key}): {err.ErrorMessage}");
                    }
                }
                return View(model);
            }


            if (PosterFile != null && PosterFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var ext = Path.GetExtension(PosterFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(ext))
                {
                    ModelState.AddModelError("PosterUrl", "Sadece JPG, PNG veya GIF formatında dosya yükleyebilirsiniz.");
                    return View(model);
                }

                var fileName = Guid.NewGuid().ToString() + ext;
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await PosterFile.CopyToAsync(stream);
                }

                model.PosterUrl = "/images/" + fileName;
            }

            _context.Movies.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var movie = _context.Movies.Find(id);
            if (movie == null)
                return NotFound();

            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Movie model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingMovie = await _context.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == model.Id);
            if (existingMovie == null)
                return NotFound();

            // Eski poster ve video url'yi koru
            model.PosterUrl = existingMovie.PosterUrl;
            model.VideoUrl = existingMovie.VideoUrl;

            _context.Movies.Update(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }



        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound();

            return View(movie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> VideoEkle(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound();

            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VideoEkle(int id, IFormFile VideoFile, IFormFile PosterFile)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound();

            // Video işlemi
            if (VideoFile != null && VideoFile.Length > 0)
            {
                var allowedVideoExtensions = new[] { ".mp4", ".mov", ".webm" };
                var ext = Path.GetExtension(VideoFile.FileName).ToLowerInvariant();
                if (!allowedVideoExtensions.Contains(ext))
                {
                    ModelState.AddModelError("VideoUrl", "Sadece MP4, MOV veya WEBM formatı destekleniyor.");
                    return View(movie);
                }

                var videoFileName = Guid.NewGuid().ToString() + ext;
                var videoFolder = Path.Combine(_webHostEnvironment.WebRootPath, "videos");
                if (!Directory.Exists(videoFolder))
                    Directory.CreateDirectory(videoFolder);

                var videoPath = Path.Combine(videoFolder, videoFileName);
                using (var stream = new FileStream(videoPath, FileMode.Create))
                    await VideoFile.CopyToAsync(stream);

                movie.VideoUrl = "/videos/" + videoFileName;
            }

            // Poster işlemi
            if (PosterFile != null && PosterFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var ext = Path.GetExtension(PosterFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(ext))
                {
                    ModelState.AddModelError("PosterUrl", "Sadece JPG, PNG veya GIF formatında dosya yükleyebilirsiniz.");
                    return View(movie);
                }

                var fileName = Guid.NewGuid().ToString() + ext;
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await PosterFile.CopyToAsync(stream);
                }

                movie.PosterUrl = "/images/" + fileName;
            }

            _context.Movies.Update(movie);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Movie", new { id = movie.Id });
        }


    }
}
