using System;
using System.ComponentModel.DataAnnotations;

namespace JulJul.Core
{
    public abstract class AbstractEntity : IEntity
    {
        [Key]
        public virtual Guid Id { get; set; }=Guid.NewGuid();

        public string GetEntityName()
        {
            return this.GetType().Name;
        }

        public string GetChannelKey()
        {
            return this.GetType().FullName;
        }
    }
}