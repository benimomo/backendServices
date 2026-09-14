using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain;
using AuthService.Infrastructure;
using BCrypt.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
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
                return Conflict(new ApiResponse<object>
                {
                    Success = false,
                    Message = AuthMessages.UsernameAlreadyExists,
                    ErrorCode = "AUTH_002"
                });
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var newUser = new User { Username = dto.Username, Email = dto.Email, PasswordHash = passwordHash };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = AuthMessages.RegistrationSuccessful
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest(new ApiResponse<LoginResponseDto>
                {
                    Success = false,
                    Message = AuthMessages.EmptyCredentials,
                    ErrorCode = "AUTH_003"
                });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return Unauthorized(new ApiResponse<LoginResponseDto>
                {
                    Success = false,
                    Message = AuthMessages.InvalidCredentials,
                    ErrorCode = "AUTH_001"
                });
            }

            var token = _tokenService.CreateToken(user.Id.ToString(), user.Username, user.Role);

            return Ok(new ApiResponse<LoginResponseDto>
            {
                Success = true,
                Message = AuthMessages.LoginSuccessful,
                Data = new LoginResponseDto
                {
                    Token = token,
                    Username = user.Username,
                    Role = user.Role.ToString()
                }
            });
        }
    }
}