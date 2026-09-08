using AuthService.Domain;

namespace AuthService.Application.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(string userId, string userName, Role role);
    }
}