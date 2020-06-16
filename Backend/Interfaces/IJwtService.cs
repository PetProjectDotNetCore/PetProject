using PetProject.Web.API.Models.DTOs;
using PetProject.Web.API.Models.Entities;

namespace PetProject.Web.API.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
        RefreshToken GenerateRefreshToken(User user);
        RefreshTokenDto RefreshTokens(string accessToken, string refreshToken);
    }
}
