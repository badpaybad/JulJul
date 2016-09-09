using System;
using System.Collections.Generic;
using JulJul.Core.Distributed;
using JulJul.Repository;

namespace JulJul.AdminBusiness.Admin
{
    public static class AdminServicesEngine
    {
        static Dictionary<Type, object> _map = new Dictionary<Type, object>();
        static DistributedServices _distributedServices = new DistributedServices();

        static AdminServicesEngine()
        {
            lock (_map)
            {
                var adminContentServices = new AdminContentServices(RepositoryEngine.Resolve<IContentRepository>());
                _map[typeof (AdminContentServices)] = adminContentServices;
                _map[typeof (AdminUserServices)] = new AdminUserServices(RepositoryEngine.Resolve<IUserRepository>(),
                    adminContentServices);
            }
        }

        public static void Boot()
        {
            foreach (var m in _map)
            {
                var s = m.Value as IAdminServciesSubscribeChange;

                s.RegisterSubscribeChange(_distributedServices);
            }
        }
    }
}