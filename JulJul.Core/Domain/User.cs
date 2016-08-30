using System.ComponentModel.DataAnnotations.Schema;

namespace JulJul.Core.Domain
{
    [Table("User")]
    public class User : AbstractEntity
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}