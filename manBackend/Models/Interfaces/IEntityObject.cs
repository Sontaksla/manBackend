using System.ComponentModel.DataAnnotations;

namespace manBackend.Models.Interfaces
{
    /// <summary>
    /// Simplifier for entity framework
    /// </summary>
    public interface IEntityObject
    {
        public int Id { get; set; }
    }
}
