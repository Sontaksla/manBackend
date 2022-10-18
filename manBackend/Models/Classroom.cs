using manBackend.Models.Attributes;
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
        [Check]
        public string Name { get; set; }
        public User Teacher { get; set; }

        public List<User> Students { get; set; }
        public List<Exercise> Exercises { get; set; }
        public Classroom()
        {
            Students = new List<User>();
            Exercises = new List<Exercise>();
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
