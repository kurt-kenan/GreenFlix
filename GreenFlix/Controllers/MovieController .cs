using GreenFlix.Data;
using GreenFlix.Models;
using GreenFlix.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace GreenFlix.Controllers
{
    public class MovieController : Controller
    {
        private readonly AppDbContext _context;

        public MovieController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? search, string? genre, string? year, double? rating)
        {
            var movies = _context.Movies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                movies = movies.Where(m => m.Title.ToLower().Contains(searchLower));
            }

            if (!string.IsNullOrWhiteSpace(genre))
            {
                var genreLower = genre.ToLower();
                movies = movies.Where(m => m.Genre.ToLower().Contains(genreLower));
            }

            if (!string.IsNullOrWhiteSpace(year))
            {
                if (year == "before1980")
                    movies = movies.Where(m => m.ReleaseDate.Year < 1980);
                else if (int.TryParse(year, out int yearValue))
                    movies = movies.Where(m => m.ReleaseDate.Year == yearValue);
            }

            if (rating.HasValue)
                movies = movies.Where(m => m.Rating >= rating.Value);

            movies = movies.OrderByDescending(m => m.Id);

            var movieList = movies.ToList();

            // Kullanıcının favorilerini çek
            var username = User?.Identity?.Name;
            List<int> favoriteMovieIds = new List<int>();
            if (!string.IsNullOrEmpty(username))
            {
                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    favoriteMovieIds = _context.FavoriteMovies
                        .Where(f => f.UserId == user.Id)
                        .Select(f => f.MovieId)
                        .ToList();
                }
            }

            // ViewModel listesi oluştur
            var model = movieList.Select(m => new MovieViewModel
            {
                Movie = m,
                IsFavorited = favoriteMovieIds.Contains(m.Id)
            }).ToList();

            return View(model);
        }

        public IActionResult Details(int id)
        {
            var username = User?.Identity?.Name;

            var movie = _context.Movies.FirstOrDefault(m => m.Id == id);
            if (movie == null)
                return NotFound();

            bool isWatched = false;
            bool isFavorited = false;

            if (!string.IsNullOrEmpty(username))
            {
                isWatched = _context.WatchedMovies
                    .Include(wm => wm.User)
                    .Any(wm => wm.User.Username == username && wm.MovieId == id);

                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    isFavorited = _context.FavoriteMovies
                        .Any(f => f.UserId == user.Id && f.MovieId == id);
                }
            }

            var comments = _context.MovieComments
                .Include(c => c.User)
                .Where(c => c.MovieId == id)
                .OrderByDescending(c => c.CreatedAt)
                .ToList();

            var model = new MovieWithWatchStatusViewModel
            {
                Movie = movie,
                IsWatched = isWatched,
                IsFavorited = isFavorited, // 🔥 Burası eklendi
                Comments = comments
            };

            return View(model);
        }


        public IActionResult Watched()
        {
            var username = User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var watchedMovies = _context.WatchedMovies
                .Include(wm => wm.Movie)
                .Include(wm => wm.User)
                .Where(wm => wm.User.Username == username)
                .Select(wm => wm.Movie)
                .ToList();

            return View(watchedMovies);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsUnwatched(int id)
        {
            var username = User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Account");

            var watchedMovie = _context.WatchedMovies
                .Include(wm => wm.User)
                .FirstOrDefault(wm => wm.User.Username == username && wm.MovieId == id);

            if (watchedMovie != null)
            {
                _context.WatchedMovies.Remove(watchedMovie);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsWatched(int id)
        {
            var username = User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Account");

            bool alreadyWatched = _context.WatchedMovies
                .Include(wm => wm.User)
                .Any(wm => wm.User.Username == username && wm.MovieId == id);

            if (!alreadyWatched)
            {
                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                if (user == null)
                    return RedirectToAction("Login", "Account");

                var watched = new WatchedMovie
                {
                    User = user,
                    MovieId = id,
                    WatchedDate = DateTime.Now
                };
                _context.WatchedMovies.Add(watched);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(int id)
        {
            var username = User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
                return Unauthorized();

            var favorite = await _context.FavoriteMovies.FirstOrDefaultAsync(f => f.UserId == user.Id && f.MovieId == id);

            if (favorite != null)
                _context.FavoriteMovies.Remove(favorite);
            else
                _context.FavoriteMovies.Add(new FavoriteMovie
                {
                    UserId = user.Id,
                    MovieId = id,
                    AddedDate = DateTime.Now
                });

            await _context.SaveChangesAsync();
            return Ok(new { isFavorite = favorite == null });
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int movieId, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return BadRequest("Yorum boş olamaz.");

            var username = User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return Unauthorized();

            var comment = new MovieComment
            {
                MovieId = movieId,
                UserId = user.Id,
                Text = text,
                CreatedAt = DateTime.Now
            };

            _context.MovieComments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = movieId });
        }

        public IActionResult Favorites()
        {
            var username = User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Account");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var favoriteMovies = _context.FavoriteMovies
                .Include(f => f.Movie)
                .Where(f => f.UserId == user.Id)
                .Select(f => f.Movie)
                .ToList();

            return View(favoriteMovies);
        }
    }
}
