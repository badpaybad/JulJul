using JulJul.Core.Distributed;

namespace JulJul.AdminBusiness.Admin
{
    internal interface IAdminServciesSubscribeChange
    {
        DistributedServices DistriubtedServices { get; set; }
        void RegisterSubscribeChange(DistributedServices distributedServices);
     
    }
}
