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

            // waiting for the task to end before getting out of constructor
            SavePasswordAsync(sb.ToString(), false).Wait();
        }
        /// <summary>
        /// Just saving in /Passwords/passes
        /// </summary>
        /// <param name="source"></param>
        /// <param name="login"></param>
        /// <returns></returns>
        public async Task SavePasswordAsync(string decryptedPassword, bool change = false) 
        {
            if (change) 
            {
                await DeletePassword(); 
            }

            await SavePassword(decryptedPassword);
        }
        private Task DeletePassword() 
        {
            string mainLoc = Assembly.GetEntryAssembly().Location.Split("\\bin")[0];

            string readPath = mainLoc + "/Passwords/passes.txt";
            string writePath = mainLoc + "/Passwords/newPasses.txt";

            if (!File.Exists(readPath)) 
            {
                Directory.CreateDirectory(mainLoc + "/Passwords");
                File.Create(readPath).Close();
                return Task.CompletedTask;
            }

            using StreamReader sr = new StreamReader(readPath);
            using StreamWriter sw = new StreamWriter(writePath);

            string line = sr.ReadLine();

            while (line != null) 
            {
                KeyValuePair<string, byte[]> pair = new KeyValuePair<string, byte[]>();

                try
                {
                    var newPair = JsonConvert.DeserializeObject<KeyValuePair<string, string>>(line);
                    // no need to use password when deleting

                    //IEnumerable<byte> asBytes = newPair.Value.Split(' ').Select(i => byte.Parse(i));
                    //pair = new KeyValuePair<string, byte[]>(newPair.Key, asBytes.ToArray());

                    pair = new KeyValuePair<string, byte[]>(newPair.Key, null);
                }
                catch {
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

            return Task.CompletedTask;
        }
        private Task SavePassword(string password) 
        {
            string mainLoc = Assembly.GetEntryAssembly().Location.Split("\\bin")[0];

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("Password has to be correct format");

            string path = mainLoc + "/Passwords/passes.txt";

            if (!File.Exists(path))
            {
                Directory.CreateDirectory(mainLoc + "/Passwords");
                File.Create(path).Close();
            }

            string json = JsonConvert.SerializeObject(
                new KeyValuePair<string, string>(Login, string.Join(' ', Encoding.UTF8.GetBytes(password)))
                );

            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteLine(json);

                sw.Close();
            }

            Password = password.HashSha256();

            return Task.CompletedTask;
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
        /// For simplicity
        /// </summary>
        /// <param name="user"></param>
        public static implicit operator bool(User user) 
        {
            return user != null;
        }
    }
}
