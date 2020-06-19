using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetProject.Application.Login;
using PetProject.Web.API.Models.DTOs;
using PetProject.Domain.Entities;

namespace PetProject.Web.API.Controllers
{
	[ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
		private const string COOKIE_NAME = "PetProject.RefreshToken";
		private readonly ILoginService _loginService;
        private readonly IJwtService _jwtService;

        public LoginController(ILoginService loginService, IJwtService jwtService)
        {
			var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
			_loginService = loginService;
            _jwtService = jwtService;
        }

        [HttpPost("authenticate")]
		public IActionResult Authenticate([FromBody]AuthenticateModel model)
        {
            var user = _loginService.Authenticate(model.Email, model.Password);

            if (user == null)
			{
				return BadRequest(new { message = "Email or password is incorrect" });
			}

			var token = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken(user);

            WriteCookie(refreshToken);

            return Ok(new
            {
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                Token = token,
                refreshToken.Expires
            });
        }

        [HttpPost]
        [Route("refresh")]
        public IActionResult Refresh([FromBody] RefreshTokenDto accessToken)
        {
            if (!HttpContext.Request.Cookies.TryGetValue(COOKIE_NAME, out string refreshToken))
            {
                return BadRequest("RefreshToken as a cookie is expected.");
            }
			if (string.IsNullOrWhiteSpace(accessToken?.AccessToken))
			{
				return BadRequest("AccessToken is expected.");
			}

			var dto = _jwtService.RefreshTokens(accessToken.AccessToken, refreshToken);

            WriteCookie(dto.RefreshToken);

            return Ok(new
            {
                Token = dto.AccessToken,
				dto.RefreshToken.Expires
            });
        }

		private void WriteCookie(RefreshToken refreshToken)
        {
            HttpContext.Response.Cookies.Append(COOKIE_NAME, refreshToken.Token, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Path = "/login",
                Secure = true,
                Expires = new System.DateTimeOffset(refreshToken.Expires)
            });
		}
	}
}