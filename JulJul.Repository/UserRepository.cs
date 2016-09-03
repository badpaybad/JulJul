using JulJul.Core;
using JulJul.Core.Domain;
using JulJul.Repository.Ef6;

namespace JulJul.Repository
{
    public class UserRepository :  EfAbstractRepository<User>, IUserRepository
    {
    }
}