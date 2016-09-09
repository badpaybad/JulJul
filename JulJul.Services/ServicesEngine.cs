using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JulJul.Core.Distributed;
using JulJul.Core.Domain;
using JulJul.Repository;

namespace JulJul.Services
{
    public static class ServicesEngine
    {
        static Dictionary<Type, object> _map = new Dictionary<Type, object>();
        public static DistributedServices DistributedServices;
        static ServicesEngine()
        {
            DistributedServices=new DistributedServices();
            lock (_map)
            {
                var contentServices = new ContentServices(RepositoryEngine.Resolve<IContentRepository>());
                _map[typeof (ContentServices)] = contentServices;
                _map[typeof (ContentDetails)] = contentServices;
                var userServices = new UserServices(RepositoryEngine.Resolve<IUserRepository>(),
                    contentServices);
                _map[typeof (UserServices)] = userServices;
                _map[typeof (UserDetails)] = userServices;
            }
        }

        public static T Resolve<T>()
        {
            object t;
            _map.TryGetValue(typeof (T), out t);
            return (T) t;
        }

        public static void Boot()
        {
           
        }
    }
}