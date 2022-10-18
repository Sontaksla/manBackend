using manBackend.Models.Attributes;
using manBackend.Models.Externsions;
using manBackend.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;

namespace manBackend.Models
{
    public class Classroom : IEntityObject
    {
        [Key]
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }
        [Check]
        public string HashId { get; private set; }
        public User Teacher { get; set; }

        public List<User> Students { get; set; }
        public List<Exercise> Exercises { get; set; }
        public Classroom(string name)
        {
            Students = new List<User>();
            Exercises = new List<Exercise>();

            Name = name;

            string str = "qwertyuiopasdfghjklzxcvbnm";

            // still a collision posibility
            for (int i = 0; i < 16; i++)
            {
                char letter = str[Random.Shared.Next(str.Length)];

                HashId += Random.Shared.Next(0, 2) == 0 ? letter : char.ToUpper(letter);
            }
        }
        public Classroom(string name, User teacher) : this(name)
        {
            Teacher = teacher;
        }
        public override bool Equals(object obj)
        {
            if (obj is not Classroom room) return false;

            return CheckAttribute.Check<Classroom>(this, room);
        }
        public override int GetHashCode()
        {
            return CheckAttribute.GetHashCode<Classroom>();
        }
        public static bool operator ==(Classroom left, Classroom right)
        {
            if (left is null && right is null) return true;
            if (left is null && right is not null) return false;

            return left.Equals(right);
        }
        public static bool operator !=(Classroom left, Classroom right)
        {
            if (left is null && right is null) return false;
            if (left is null && right is not null) return true;

            return !left.Equals(right);
        }
    }
}
