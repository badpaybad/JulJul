using System;
using System.Collections.Generic;
using JulJul.Core.Domain;

namespace JulJul.Repository
{
    public static class RepositoryFactor
    {
        static Dictionary<Type, object> _map = new Dictionary<Type, object>();
      
        static RepositoryFactor()
        {
            lock (_map)
            {
                var userRepository = new UserRepository();
                _map[typeof(User)] = userRepository;
                _map[typeof(IUserRepository)] = userRepository;

                var contentRepository = new ContentRepository();
                _map[typeof(Content)] = contentRepository;
                _map[typeof(IContentRepository)] = contentRepository;
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