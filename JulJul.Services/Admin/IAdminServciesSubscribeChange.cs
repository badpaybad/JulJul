using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JulJul.Core.Distributed;

namespace JulJul.Services.Admin
{
    internal interface IAdminServciesSubscribeChange
    {
        void RegisterSubscribeChange(DistributedServices distributedServices);
     
    }
}
