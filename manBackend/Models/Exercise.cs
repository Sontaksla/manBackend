using manBackend.Models.Attributes;
using manBackend.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace manBackend.Models
{
    public class Exercise : IEntityObject
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Check]
        public string Title { get; set; }
        [Check]
        public string Description { get; set; }
        [Check]
        public DateTime Expires { get; init; }

        public override bool Equals(object obj)
        {
            if (obj is not Exercise exercise) return false;

            return CheckAttribute.Check<Exercise>(this, exercise);
        }
        public override int GetHashCode()
        {
            return CheckAttribute.GetHashCode<Exercise>();
        }
    }
}
