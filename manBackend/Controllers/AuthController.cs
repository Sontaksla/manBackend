using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using manBackend.Models;
using System.Net.Mail;
using Microsoft.AspNetCore.Authorization;
using manBackend.Models.Externsions;

namespace manBackend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly BackendDbContext _context;
        public AuthController(IConfiguration configuration, BackendDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        [HttpPost]
        public IActionResult Register([FromBody]User user) 
        {
            var checkResult = user.CheckUser();

            if (checkResult is BadRequestObjectResult) 
            {
                return checkResult;
            }

            User sameUser = _context.Users.FirstOrDefault(i => i.Login == user.Login || i.Mail.Address == user.Mail.Address);

            if (sameUser) 
            {
                return BadRequest(new 
                {
                    Login = sameUser.Login,
                    Mail = sameUser.Mail.Address
                });
            }
            // mail verification code
            Task<int> verifyCodeTask = user.Mail.SendVerificationMailAsync(_configuration);

            return Ok(verifyCodeTask.Result);
        }
        [HttpPost("VerifyReg")]
        [Authorize]
        public IActionResult VerifyRegistration([FromBody]User user) 
        {
            var checkResult = user.CheckUser();

            if (checkResult is BadRequestObjectResult)
            {
                return checkResult;
            }

            user.SavePasswordAsync(user.Password);

            _context.Users.Add(user);
            _context.SaveChangesAsync();

            string token = GetToken(user);

            return Ok(token);
        }
        private string GetToken(User user) 
        {
            Claim[] claims = 
            {
                new Claim(ClaimTypes.Name, user.UserName), 
                new Claim(ClaimTypes.NameIdentifier, user.Login),
                new Claim(ClaimTypes.Email, user.Mail.Address)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

            var handler = new JwtSecurityTokenHandler();
            var descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),

                Audience = _configuration["JWT:Audience"],
                Issuer = _configuration["JWT:Issuer"],
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.Sha256)
            };

            var token = handler.CreateToken(descriptor);

            return handler.WriteToken(token);
        }
    }
}
