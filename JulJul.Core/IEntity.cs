using System.ComponentModel.DataAnnotations;

namespace JulJul.Core
{
    public interface IEntity
    {
        [Key]
        long Id { get; set; }

        string EntityName();
        string GetEntityName();
    }
}