using System.ComponentModel.DataAnnotations;

namespace JulJul.Core
{
    public abstract class AbstractEntity : IEntity
    {
        [Key]
        public virtual long Id { get; set; }

        public string EntityName()
        {
            return this.GetType().Name;
        }

        public string GetEntityName()
        {
            return this.GetType().Name;
        }
    }
}