using manBackend.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;

namespace manBackend.Models.Auth
{
    public class Email : IEntityObject
    {
        [Key]
        [Required]
        public int Id { get; set; }

        public string Address { get; set; }
        public Email(string address)
        {
            Address = address;
        }
        public Task<int> SendVerificationMailAsync(IConfiguration config) 
        {
            return SendVerificationMailAsync(Address, config);
        }
        private async Task<int> SendVerificationMailAsync(string address, IConfiguration config) 
        {
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.Credentials = new NetworkCredential(config["Mail:UserName"], config["Mail:Password"]);

            int code = Random.Shared.Next();

            await client.SendMailAsync(new MailMessage(config["Mail:UserName"], address) 
            {
                Body = "Confirm mail code: " + code,
            });

            return code;
        }
    }
}
