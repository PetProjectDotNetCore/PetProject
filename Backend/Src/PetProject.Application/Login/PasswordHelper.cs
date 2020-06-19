using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PetProject.Application.Login
{
    public static class PasswordHelper
    {
        public static (byte[], byte[]) CreatePasswordHash(string password)
        {
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			if (string.IsNullOrWhiteSpace(password))
			{
				throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
			}

            using var hmac = new HMACSHA512();
            return (hmac.Key, hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        public static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			if (string.IsNullOrWhiteSpace(password))
			{
				throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
			}

            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                if (computedHash.Where((hash, index) => hash != storedHash[index]).Any())
                {
                    return false;
                }
            }

            return true;
        }
    }
}
