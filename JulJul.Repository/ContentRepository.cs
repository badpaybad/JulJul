using JulJul.Core;
using JulJul.Core.Domain;
using JulJul.Repository.Ef6;

namespace JulJul.Repository
{
    public class ContentRepository : EfAbstractRepository<Content>,IContentRepository
    {
    }
}