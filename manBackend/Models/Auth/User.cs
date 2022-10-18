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
using Newtonsoft.Json;
using System.Net;
using System.IO.Pipes;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using manBackend.Models.Externsions;

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
        public List<Classroom> Rooms { get; set; }
        /// <summary>
        /// Encrypted password
        /// </summary>

        public string Password { get; set; }
        /// <summary>
        /// Empty by default
        /// </summary>
        public User()
        {
            Mail = new Email("");
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

            SavePasswordAsync(sb.ToString(), false);
        }
        /// <summary>
        /// Just saving in /Passwords/passes
        /// </summary>
        /// <param name="source"></param>
        /// <param name="login"></param>
        /// <returns></returns>
        public Task SavePasswordAsync(string decryptedPassword, bool change = false) 
        {
            if (change) DeletePassword();

            return SavePasswordAsync(decryptedPassword);
        }
        private void DeletePassword() 
        {
            string readPath = Environment.CurrentDirectory + "/Passwords/passes.txt";
            string writePath = Environment.CurrentDirectory + "/Passwords/newPasses.txt";

            using StreamReader sr = new StreamReader(readPath);
            using StreamWriter sw = new StreamWriter(writePath);

            string line = sr.ReadLine();

            while (line != null) 
            {
                KeyValuePair<string, byte[]> pair = new KeyValuePair<string, byte[]>();

                try {
                    pair = JsonConvert.DeserializeObject<KeyValuePair<string, byte[]>>(line);
                } catch {
                    line = sr.ReadLine();
                    continue; 
                }

                if (Login != pair.Key)
                {
                    sw.WriteLine(line);
                }

                line = sr.ReadLine();
            }
            sr.Close();
            sw.Close();

            File.Delete(readPath);
            File.Move(writePath, readPath);

        }
        private async Task SavePasswordAsync(string password) 
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("Password has to be correct format");

            string path = Environment.CurrentDirectory + "/Passwords/passes.txt";

            string json = JsonConvert.SerializeObject(
                new KeyValuePair<string, string>(Login, string.Join(' ', Encoding.UTF8.GetBytes(password)))
                );

            using (StreamWriter sw = new StreamWriter(path, true))
            {
                await sw.WriteLineAsync(json);

                sw.Close();
            }

            Password = password.HashSha256();
        }
        /// <summary>
        /// Checks if two users are equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is not User objAsUser) return false;

            return CheckAttribute.Check<User>(this, objAsUser);
        }
        public override int GetHashCode()
        {
            return CheckAttribute.GetHashCode<User>();
        }
        public static bool operator ==(User left, User right) 
        {
            if (left is null && right is null) return true;
            if (left is null && right is not null) return false;

            return left.Equals(right);
        }
        public static bool operator !=(User left, User right)
        {
            if (left is null && right is null) return false;
            if (left is null && right is not null) return true;

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
