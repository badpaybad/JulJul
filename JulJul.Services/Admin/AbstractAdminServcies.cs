using JulJul.Core;
using JulJul.Core.Distributed;

namespace JulJul.Services.Admin
{
    internal abstract class AbstractAdminServcies<T, TView> : IAdminServices<T, TView>, IAdminServciesSubscribeChange
        where T : IEntity, new() where TView : AbstractDetails<T, TView>, new()
    {
        public void RegisterSubscribeChange(DistributedServices distributedServices)
        {
            distributedServices.EntityDetailsSubcribe<T, TView>((channel, cmd) =>
            {
                switch (cmd.CommandType)
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