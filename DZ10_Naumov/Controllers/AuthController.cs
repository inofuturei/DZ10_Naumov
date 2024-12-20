using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System.Threading.Tasks;
using DZ10_Naumov.Models;

public class AuthController : Controller
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Register(string username, string password, string email)
    {
        try
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User { Username = username, PasswordHash = passwordHash, Email = email };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (DbUpdateException ex)
        {
            // Логирование ошибки
            return StatusCode(500, "Internal server error");
        }
        catch (Exception ex)
        {
            // Логирование ошибки
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return Unauthorized();
            }
            // Здесь можно добавить логику для создания токена или сессии
            return Ok();
        }
        catch (Exception ex)
        {
            // Логирование ошибки
            return StatusCode(500, "Internal server error");
        }
    }
}