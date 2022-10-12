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
using Microsoft.EntityFrameworkCore;
using manBackend.Models.Auth;
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
        /// <summary>
        /// Checks <paramref name="user"/> and sends email verification message
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Register([FromForm]User user) 
        {
            IActionResult checkResult = user.CheckUser();

            if (checkResult is BadRequestObjectResult) 
            {
                return checkResult;
            }

            User sameUser = _context.Users.Include(i => i.Mail).FirstOrDefault(i => i.Login == user.Login || i.Mail.Address == user.Mail.Address);

            if (sameUser) 
            {
                return BadRequest(new 
                {
                    Login = sameUser.Login,
                    Mail = sameUser.Mail.Address
                });
            }
            //Google removed less-secure apps option

            //int verifyCode = await user.Mail.SendVerificationMailAsync(_configuration);

            string token = GetToken(user);

            return Ok(token);
        }
        /// <summary>
        /// Adds <paramref name="user"/> to the database and saves password
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VerifyRegistration([FromForm]User user) 
        {
            var checkResult = user.CheckUser();

            if (checkResult is BadRequestObjectResult)
            {
                return checkResult;
            }

            await user.SavePasswordAsync(user.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok();
        }
        [HttpPost]
        public IActionResult Login([FromForm]User user)
        {
            if (user.Login.IsNullOrEmpty() || user.Login.Length < 4)
                return BadRequest(user.Login);
            if (user.Password.IsNullOrEmpty() || user.Password.Length < 4)
                return BadRequest(user.Password);

            string hashedPass = user.Password.HashSha256();

            User same = _context.Users.Include(i => i.Mail).FirstOrDefault(i => i.Login == user.Login && i.Password == hashedPass);

            if (!same) 
            {
                return NotFound("No user found");
            }

            string token = GetToken(same);

            return Ok(token);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromForm]string newPassword) 
        {
            if (newPassword.IsNullOrEmpty() || newPassword.Length < 4
                // user password contains non-ASCII symbols 
                || Encoding.UTF8.GetBytes(newPassword).Length > newPassword.Length)
                return BadRequest("New password has to be correct format");

            Claim login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);

            if (login == null) 
            {
                return BadRequest("Login error");
            }

            User user = _context.Users.Include(i => i.Mail).FirstOrDefault(i => i.Login == login.Value);

            if (user == null) 
            {
                return BadRequest("Login error");
            }

            await user.SavePasswordAsync(newPassword, true);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok();
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
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };

            var token = handler.CreateToken(descriptor);

            return handler.WriteToken(token);
        }
    }
}
