using manBackend.Models.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net.Mail;
using System.Text;

namespace manBackend.Models.Externsions
{
    public static class UserExtensions
    {
        public static IActionResult CheckUser(this User user) 
        {
            if (user.UserName.IsNullOrEmpty() || user.UserName.Length < 4)
                return new BadRequestObjectResult(user.UserName);

            if (user.Login.IsNullOrEmpty() || user.Login.Length < 4)
                return new BadRequestObjectResult(user.Login);

            if (!MailAddress.TryCreate(user.Mail.Address, out _))
                return new BadRequestObjectResult(user.Mail.Address);

            if (user.Password.IsNullOrEmpty() || user.Password.Length < 4
                // user password contains non-ASCII symbols 
                || Encoding.UTF8.GetBytes(user.Password).Length > user.Password.Length)
                return new BadRequestObjectResult(user.Password);

            return new OkObjectResult(user);
        }
    }
}
