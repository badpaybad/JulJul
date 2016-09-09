using System;
using System.ComponentModel.DataAnnotations;

namespace JulJul.Core
{
    public interface IEntity
    {
        [Key]
        Guid Id { get; set; }

     
        string GetEntityName();
        string GetChannelKey();
    }
}