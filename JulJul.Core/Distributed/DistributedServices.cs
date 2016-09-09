using System;
using JulJul.Core.Redis;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace JulJul.Core.Distributed
{
    public class DistributedServices
    {
        static DistributedServices _instance=new DistributedServices();
        public static DistributedServices Instance { get { return _instance; } }

        private ISubscriber _subscriber;

        private DistributedServices()
        {
            RedisConnectionPool.Boot(new RedisConfig()
            {
                Host = "badpaybad.info",
                Port = 6379,
                Pwd = "badpaybad.info"
            });

            _subscriber = RedisConnectionPool.CurrentConnectionMultiplexer.GetSubscriber();
        }
        public void DbSubcribe<T>(Action<string,DistributedDbCommand<T>> callBack) where T : IEntity
        {
            _subscriber.Subscribe(typeof(T).FullName, (channel, value) =>
            {
                var cmd = JsonConvert.DeserializeObject<DistributedDbCommand<T>>(value);
                callBack(channel, cmd);
            });
        }

        public void DbPublish<T>(DistributedDbCommand<T> cmd) where T : IEntity
        {
            _subscriber.Publish(typeof (T).FullName, cmd.ToJson());
        }

        public void EntityDetailsPublish<T, TView>(DistributedEntityDetailsCommand<T, TView> cmd)
            where T : IEntity, new() where TView : AbstractDetails<T, TView>
        {
            _subscriber.Publish(typeof(TView).FullName, cmd.ToJson());
        }

        public void EntityDetailsSubcribe<T, TView>(Action<string,DistributedEntityDetailsCommand<T, TView>> callBack )
            where T : IEntity, new() where TView : AbstractDetails<T, TView>
        {
            _subscriber.Subscribe(typeof(TView).FullName, (channel, value) =>
            {
                var cmd = JsonConvert.DeserializeObject<DistributedEntityDetailsCommand<T,TView>>(value);
                callBack(channel, cmd);
            });
        }
    }
}