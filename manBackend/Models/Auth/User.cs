using manBackend.Models.Attributes;
using manBackend.Models.Interfaces;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using System.IO.Pipes;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace manBackend.Models.Auth
{
    public sealed class User : IEntityObject
    {
        [Key]
        [Required]
        public int Id { get; set; }

        public string UserName { get; set; }
        [Check]
        public string Login { get; set; }
        [Check]
        public Email Mail { get; set; }
        /// <summary>
        /// Encrypted password
        /// </summary>

        public string Password { get; private set; }
        /// <summary>
        /// Empty by default
        /// </summary>
        public User()
        {
        }
        public User(string userName, string login, string address) 
        {
            if (!MailAddress.TryCreate(address, out MailAddress result))
                throw new ArgumentException(nameof(address) + " has to be correct mail format");
            if (string.IsNullOrEmpty(login))
                throw new ArgumentException(nameof(login) + " has to be correct format");
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentException(nameof(userName) + " has to be correct format");

            Mail = new Email(result.Address);
            UserName = userName;
            Login = login;

        }
        /// <summary>
        /// <paramref name="ptr"/> has to be pointer to the password
        /// </summary>
        /// <param name="ptr"></param>
        public unsafe User(string userName, string login, string address, char* ptr, int length) 
            : this(userName, login, address)
        {
            StringBuilder sb = new StringBuilder(length);
            for (int i = 0; i < length; i++, ptr++)
            {
                sb.Append(*ptr);
            }

            SavePasswordAsync(sb.ToString());
        }
        /// <summary>
        /// Just saving in /Passwords/passes
        /// </summary>
        /// <param name="source"></param>
        /// <param name="login"></param>
        /// <returns></returns>
        public Task SavePasswordAsync(string password) => 
            SavePasswordAsync(password, true);
        private async Task SavePasswordAsync(string password, bool append) 
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("Password has to be correct format");

            byte[] bytes = Encoding.UTF8.GetBytes(password);

            byte[] hashBytes = SHA256.HashData(bytes);

            StringBuilder sb = new StringBuilder(hashBytes.Length);

            foreach (var byte_ in hashBytes)
            {
                sb.Append(byte_.ToString("x2")); // (hexadecimal)
            }

            Password = sb.ToString();

            Console.WriteLine(Environment.CurrentDirectory);

            string path = Environment.CurrentDirectory + "/Passwords/passes.txt";

            string json = JsonConvert.SerializeObject(
                new KeyValuePair<string, string>(Login, string.Join(' ', bytes))
                );

            using (StreamWriter sw = new StreamWriter(path, append))
            {
                await sw.WriteLineAsync(json);

                sw.Close();
            }
        }
        /// <summary>
        /// Checks if two users are equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null || obj is not User objAsUser) return false;

            return CheckAttribute.Check<User>(this, objAsUser);
        }
        public override int GetHashCode()
        {
            return CheckAttribute.GetHashCode<User>();
        }
        public static bool operator ==(User left, User right) 
        {
            return left.Equals(right);
        }
        public static bool operator !=(User left, User right)
        {
            return !left.Equals(right);
        }
        /// <summary>
        /// For simplicity in if statements
        /// <code>
        /// if(<paramref name="user"/>) {}
        /// </code>
        /// Instead of 
        /// <code>
        /// if(<paramref name="user"/> != null) {}
        /// </code>
        /// </summary>
        /// <param name="user"></param>
        public static implicit operator bool(User user) 
        {
            return user != null;
        }
    }
}
