using PetProject.Web.API.Models.DTOs;
using PetProject.Web.API.Models.Entities;

namespace PetProject.Web.API.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken(User user);
        RefreshTokenDto UpdateRefreshToken(RefreshTokenDto model);
    }
}
