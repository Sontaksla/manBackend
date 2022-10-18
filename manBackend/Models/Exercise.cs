using manBackend.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace manBackend.Models
{
    public class Exercise : IEntityObject
    {
        [Key]
        [Required]
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime Expires { get; init; }
    }
}
