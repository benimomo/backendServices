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
using Microsoft.Extensions.Caching.Memory;

namespace AuthService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly AuthDbContext _context;
        private readonly ILogger<AuthController> _logger;
        private IMemoryCache _cache;

        public AuthController(ITokenService tokenService, AuthDbContext context, ILogger<AuthController> logger, IMemoryCache cache)
        {
            _tokenService = tokenService;
            _context = context;
            _logger = logger;
            _cache = cache;
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

            var accessToken = _tokenService.CreateToken(user.Id.ToString(), user.Username, user.Role);
            var refreshToken = _tokenService.GenerateRefreshToken();
            _cache.Set(refreshToken, user.Id.ToString(), TimeSpan.FromDays(7));

            return Ok(new ApiResponse<LoginResponseDto>
            {
                Success = true,
                Message = AuthMessages.LoginSuccessful,
                Data = new LoginResponseDto
                {
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    Username = user.Username,
                    Role = user.Role.ToString()
                }
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenRequestDto dto)
        {
            if (!_cache.TryGetValue(dto.RefreshToken, out string? userId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = AuthMessages.InvalidRefreshToken,
                    ErrorCode = "AUTH_004"
                });
            }

            var user = await _context.Users.FindAsync(int.Parse(userId!));
            if (user == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = AuthMessages.UserNotFound, 
                    ErrorCode = "AUTH_001"
                });
            }

            _cache.Remove(dto.RefreshToken);

            var newAccessToken = _tokenService.CreateToken(user.Id.ToString(), user.Username, user.Role);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            _cache.Set(newRefreshToken, user.Id.ToString(), TimeSpan.FromDays(7));

            return Ok(new ApiResponse<LoginResponseDto>
            {
                Success = true,
                Message = AuthMessages.TokenRefreshed, 
                Data = new LoginResponseDto
                {
                    Token = newAccessToken,
                    RefreshToken = newRefreshToken,
                    Username = user.Username,
                    Role = user.Role.ToString()
                }
            });
        }

        [HttpPost("logout")]
        public IActionResult logout(RefreshTokenRequestDto dto)
        {
            if (!_cache.TryGetValue(dto.RefreshToken, out string? userId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = AuthMessages.InvalidRefreshToken,
                    ErrorCode = "AUTH_004"
                });
            }

            _cache.Remove(dto.RefreshToken);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = AuthMessages.LogoutSuccessful
            });
        }
    }
}