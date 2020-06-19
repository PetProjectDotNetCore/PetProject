using PetProject.Domain.Entities;

namespace PetProject.Application.Login
{
    public interface ILoginService
    {
        User Authenticate(string email, string password);
    }
}
