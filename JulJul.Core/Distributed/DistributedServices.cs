using System;
using JulJul.Core.Redis;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace JulJul.Core.Distributed
{
    public class DistributedServices
    {
        //static DistributedServices _instance=new DistributedServices();

        //public static DistributedServices Instance
        //{
        //    get
        //    {
        //        return _instance;
        //    }
        //}

        private ISubscriber _subscriber;

        public DistributedServices()
        {
            var configRedisInstance = new RedisConfig()
            {
                Host = "badpaybad.info",
                Port = 6379,
                Pwd = "badpaybad.info"
            };
            RedisConnectionPool.Boot(configRedisInstance);

            _subscriber = RedisConnectionPool.CurrentConnectionMultiplexer.GetSubscriber();
        }
        public void DbSubcribe<T>(Action<string,DistributedDbCommand<T>> callBack) where T : IEntity
        {
            _subscriber.Subscribe(typeof(T).FullName, (channel, value) =>
            {
                var cmd = JsonConvert.DeserializeObject<DistributedDbCommand<T>>(value);
                callBack(channel, cmd);

                Console.WriteLine("DbCommand:done:" + channel + "\r\n" + value);
            });
        }

        public void DbPublish<T>(DistributedDbCommand<T> cmd) where T : IEntity
        {
            var redisValue = cmd.ToJson();
            var redisChannel = typeof (T).FullName;
            _subscriber.Publish(redisChannel, redisValue);

            Console.WriteLine("DbCommand:pushed:"+redisChannel+"\r\n"+ redisValue);
        }

        public void EntityDetailsPublish<T, TView>(DistributedEntityDetailsCommand<T, TView> cmd)
            where T : IEntity, new() where TView : AbstractDetails<T, TView>
        {
            var redisChannel = typeof(TView).FullName;
            var redisValue = cmd.ToJson();
            _subscriber.Publish(redisChannel, redisValue);

            Console.WriteLine("FrontEndCommand:pushed:" + redisChannel + "\r\n" + redisValue);
        }

        public void EntityDetailsSubcribe<T, TView>(Action<string,DistributedEntityDetailsCommand<T, TView>> callBack )
            where T : IEntity, new() where TView : AbstractDetails<T, TView>
        {
            _subscriber.Subscribe(typeof(TView).FullName, (channel, value) =>
            {
                var cmd = JsonConvert.DeserializeObject<DistributedEntityDetailsCommand<T,TView>>(value);
                callBack(channel, cmd);
                Console.WriteLine("FrontEndCommand:done:" + channel + "\r\n" + value);
            });
        }
    }
}