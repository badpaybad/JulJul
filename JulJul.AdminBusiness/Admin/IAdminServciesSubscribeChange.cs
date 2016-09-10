using JulJul.Core.Distributed;

namespace JulJul.AdminBusiness.Admin
{
    internal interface IAdminServciesSubscribeChange
    {
        IDistributedServices DistriubtedServices { get; set; }
        void RegisterSubscribeChange(IDistributedServices distributedServices);
     
    }
}
