using PetProject.Domain.Entities;
using PetProject.Persistence.Interfaces;
using System;
using System.Linq;

namespace PetProject.Application.Login
{
	public class LoginService : ILoginService
    {
        private readonly IDataContext _context;

        public LoginService(IDataContext context)
        {
            _context = context;
        }

        public User Authenticate(string email, string password)
        {
			if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
			{
				return null;
			}

            var user = _context.Users.FirstOrDefault(x => x.Email == email);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            return !PasswordHelper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt) ? null : user;
        }
    }
}
