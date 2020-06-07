using Microsoft.AspNetCore.Mvc;
using PetProject.Web.API.Interfaces;
using PetProject.Web.API.Models.DTOs;

namespace PetProject.Web.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly IJwtService _jwtService;

        public LoginController(ILoginService loginService, IJwtService jwtService)
        {
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

            return Ok(new
            {
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                Token = token,
                RefreshToken = refreshToken
            });
        }

        [HttpPost]
        [Route("refresh")]
        public ActionResult<RefreshTokenDto> Refresh([FromBody]RefreshTokenDto model)
        {
            return _jwtService.UpdateRefreshToken(model);
        }
    }
}