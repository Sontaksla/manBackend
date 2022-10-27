using manBackend.Models.Attributes;
using manBackend.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public DateTime Expires { get; init; }
        public Exercise()
        {
        }
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
