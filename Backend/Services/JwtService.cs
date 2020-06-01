using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PetProject.Web.API.Configurations.Settings;
using PetProject.Web.API.Data;
using PetProject.Web.API.Interfaces;
using PetProject.Web.API.Models.DTOs;
using PetProject.Web.API.Models.Entities;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PetProject.Web.API.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly DataContext _context;

        public JwtService(IOptions<JwtSettings> jwtSettings, DataContext context)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;
        }

        public string GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.ExpirationInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken(User user)
        {
            var refreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                UserId = user.Id
            };

            _context.RefreshTokens.Add(refreshToken);
            _context.SaveChanges();

            return refreshToken.Token;
        }

        public RefreshTokenDto UpdateRefreshToken(RefreshTokenDto model)
        {
            var principal = GetPrincipalFromToken(model.AccessToken, _jwtSettings.Secret);
            var userEmail = principal.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userEmail))
            {
                throw new SecurityTokenException($"Missing claim: {ClaimTypes.Email}!");
            }

            var user = _context.Users.FirstOrDefault(x => x.Email == userEmail);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            if (!HasValidRefreshToken(model.RefreshToken, user.Id))
            {
                throw new Exception("Token is invalid");
            }

            var accessToken = GenerateAccessToken(user);
            RemoveRefreshToken(model.RefreshToken, user.Id); 
            var refreshToken = GenerateRefreshToken(user);

            return new RefreshTokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        #region private methods
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token, string signingKey)
        {
            return ValidateAccessToken(token, new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                ValidateLifetime = false // we check expired tokens here
            });
        }

        private ClaimsPrincipal ValidateAccessToken(string token, TokenValidationParameters tokenValidationParameters)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token!");
            }

            return principal;
        }

        private bool HasValidRefreshToken(string refreshToken, int userId)
        {
            return _context.RefreshTokens.Any(rt => rt.Token == refreshToken && DateTime.UtcNow <= rt.Expires && rt.UserId == userId);
        }

        private void RemoveRefreshToken(string refreshToken, int userId)
        {
            var token = _context.RefreshTokens.FirstOrDefault(x => x.UserId == userId && x.Token == refreshToken);

            if (token != null)
            {
                _context.RefreshTokens.Remove(token);
                _context.SaveChanges();
            }
        }
        #endregion
    }
}
