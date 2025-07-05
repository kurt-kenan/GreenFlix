using GreenFlix.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GreenFlix.Data;
public class UserController : Controller
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }
    public IActionResult Settings()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            return RedirectToAction("Login", "Account");

        int userId = int.Parse(userIdClaim);

        var user = _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new Users
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                IsAdmin = u.IsAdmin
            }).FirstOrDefault();

        if (user == null)
            return NotFound();

        return View(user);
    }

    public IActionResult Profile()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            return RedirectToAction("Login", "Account");

        int userId = int.Parse(userIdClaim);

        var user = _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new Users
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                IsAdmin = u.IsAdmin
            }).FirstOrDefault();

        if (user == null)
            return NotFound();

        return View(user);
    }
}
