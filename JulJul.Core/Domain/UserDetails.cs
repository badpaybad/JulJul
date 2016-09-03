namespace JulJul.Core.Domain
{
    public class UserDetails : AbstractDetails<User,UserDetails>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        
    }
}