using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ReadLater5.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ReadLater5.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _iConfiguration;

        public AuthenticationController(UserManager<IdentityUser> userManager, IConfiguration iConfiguration)
        {
            _userManager = userManager;
            _iConfiguration = iConfiguration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserModel model)
        {
            var userCheck = await _userManager.FindByNameAsync(model.Username);
            if (userCheck != null)
            {
                return BadRequest("Username is taken.");
            }

            var user = new IdentityUser()
            {
                UserName = model.Username,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok();
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
                return BadRequest("Incorrect username.");

            var checkCredentials = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!checkCredentials)
            {
                return Unauthorized("Incorrect credentials.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_iConfiguration["JWT:Key"]);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(60),
                Issuer = _iConfiguration["JWT:Issuer"],
                Audience = _iConfiguration["JWT:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(tokenHandler.WriteToken(token));
        }
    }
}
