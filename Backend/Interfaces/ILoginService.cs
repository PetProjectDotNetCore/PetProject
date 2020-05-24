using PetProject.Web.API.Models.Entities;

namespace PetProject.Web.API.Interfaces
{
    public interface ILoginService
    {
        User Authenticate(string email, string password);
    }
}
