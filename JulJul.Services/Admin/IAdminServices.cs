using JulJul.Core;

namespace JulJul.Services.Admin
{
    internal interface IAdminServices<T, TView>
        where T : IEntity, new() where TView : AbstractDetails<T, TView>, new()
    {
        void Create(TView data);
        void Update(TView data);
        void Delete(TView data);
    }
}