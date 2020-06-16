using PetProject.Web.API.Models.Entities;

namespace PetProject.Web.API.Models.DTOs
{
    public class RefreshTokenDto
    {
        public string AccessToken { get; set; }
        public RefreshToken RefreshToken { get; set; }
    }
}
