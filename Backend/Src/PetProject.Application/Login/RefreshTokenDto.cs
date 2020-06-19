using PetProject.Domain.Entities;

namespace PetProject.Application.Login
{
    public class RefreshTokenDto
    {
        public string AccessToken { get; set; }
        public RefreshToken RefreshToken { get; set; }
    }
}
