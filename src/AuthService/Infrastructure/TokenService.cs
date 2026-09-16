using AuthService.Application.Interfaces;
using AuthService.Domain;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Unicode;
using System.Security.Cryptography;
namespace AuthService.Infrastructure
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateToken(string userId, string userName, Role role)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.Role, role.ToString())
        };
            
            var expire_time = DateTime.UtcNow.AddMinutes(1);//expire access token
            var authService_Audience = _configuration["Jwt:Audience"];
            var authService_issuer = _configuration["Jwt:Issuer"];
            var authService_secretKey = _configuration["Jwt:Key"];
            var secretKeyB = Encoding.UTF8.GetBytes(authService_secretKey);
            SymmetricSecurityKey secretKeyO = new SymmetricSecurityKey(secretKeyB);

            SigningCredentials credintals = new SigningCredentials(secretKeyO, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(issuer: authService_issuer, audience: authService_Audience, claims: claims, expires: expire_time, signingCredentials: credintals);
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);

        }

        public string GenerateRefreshToken() {

            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }

    }
}







