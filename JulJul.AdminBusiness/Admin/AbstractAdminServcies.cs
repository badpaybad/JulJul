using JulJul.Core;
using JulJul.Core.Distributed;

namespace JulJul.AdminBusiness.Admin
{
    internal abstract class AbstractAdminServcies<T, TView> : IAdminServices<T, TView>, IAdminServciesSubscribeChange
        where T : IEntity, new() where TView : AbstractDetails<T, TView>, new()
    {
        public IDistributedServices DistriubtedServices { get; set; }

        public virtual void RegisterSubscribeChange(IDistributedServices distributedServices)
        {
            DistriubtedServices = distributedServices;

            DistriubtedServices.EntityDetailsSubcribe<T, TView>((channel, cmd) =>
            {
                switch (cmd.DbCommandType)
                {
                    case DistributedDbCommandType.Add:
                        Create(cmd.Data);
                        break;
                    case DistributedDbCommandType.Update:
                        Update(cmd.Data);
                        break;
                    case DistributedDbCommandType.Delete:
                        Delete(cmd.Data);
                        break;
                }
            });
        }

        public abstract void Create(TView data);
        public abstract void Update(TView data);
        public abstract void Delete(TView data);
    }
}