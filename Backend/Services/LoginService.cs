using PetProject.Web.API.Data;
using PetProject.Web.API.Helper;
using PetProject.Web.API.Interfaces;
using PetProject.Web.API.Models.Entities;
using System;
using System.Linq;

namespace PetProject.Web.API.Services
{
    public class LoginService : ILoginService
    {
        private readonly DataContext _context;

        public LoginService(DataContext context)
        {
            _context = context;
        }

        public User Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.Users.FirstOrDefault(x => x.Email == email);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            return !PasswordHelper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt) ? null : user;
        }
    }
}
