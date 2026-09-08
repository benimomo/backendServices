using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain;
using AuthService.Infrastructure;
using BCrypt.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
namespace AuthService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly AuthDbContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ITokenService tokenService, AuthDbContext context, ILogger<AuthController> logger)
        {
            _tokenService = tokenService;
            _context = context;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            {
                return BadRequest("Username already exists");
            }
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var newUser = new User { Username = dto.Username, Email = dto.Email, PasswordHash = passwordHash };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user==null)
            {
                return Unauthorized("Invalid username or password");
            }

            var hashPasswordIsMatch = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!hashPasswordIsMatch)
            {
                return Unauthorized("Invalid username or password");
            }
            var token = _tokenService.CreateToken(user.Id.ToString(), user.Username, user.Role);
            return Ok(new { Token = token });

        }



    }
}