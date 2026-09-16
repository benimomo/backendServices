using Microsoft.Extensions.Primitives;

namespace AuthService.Application.DTOs
{
    public class RefreshTokenRequestDto
    {
        public required string RefreshToken { get; set; }
    }
}
