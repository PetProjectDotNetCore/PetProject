using PetProject.Domain.Entities;

namespace PetProject.Application.Login
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
        RefreshToken GenerateRefreshToken(User user);
        RefreshTokenDto RefreshTokens(string accessToken, string refreshToken);
    }
}
