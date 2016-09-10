using System;
using JulJul.Core.Redis;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace JulJul.Core.Distributed
{
    public interface IDistributedServices
    {
        void EntitySubcribe<T>(Action<string, DistributedEntityCommand<T>> callBack) where T : IEntity;
        void EntityPublish<T>(DistributedEntityCommand<T> cmd) where T : IEntity;

        void EntityDetailsPublish<T, TView>(DistributedEntityDetailsCommand<T, TView> cmd)
            where T : IEntity, new() where TView : AbstractDetails<T, TView>;

        void EntityDetailsSubcribe<T, TView>(Action<string, DistributedEntityDetailsCommand<T, TView>> callBack)
            where T : IEntity, new() where TView : AbstractDetails<T, TView>;

        void Publish<T>(DistributedCommand<T> cmd) where T : class;
        void Subscribe<T>(Action<string, DistributedCommand<T>> callBack) where T : class;

    }
    public class DistributedServices: IDistributedServices
    {
        //static DistributedServices _instance = new DistributedServices();

        //public static DistributedServices Instance
        //{
        //    get
        //    {
        //        return _instance;
        //    }
        //}

        private ISubscriber _subscriberDb;
        private ISubscriber _subscriber;
        private ISubscriber _subscriberDetails;

        public DistributedServices()
        {
            var configRedisInstance = new RedisConfig()
            {
                Host = "badpaybad.info",
                Port = 6379,
                Pwd = "badpaybad.info"
            };
            RedisConnectionPool.Boot(configRedisInstance);

            _subscriberDb = RedisConnectionPool.CurrentConnectionMultiplexer.GetSubscriber();
            _subscriber = RedisConnectionPool.CurrentConnectionMultiplexer.GetSubscriber();
            _subscriberDetails = RedisConnectionPool.CurrentConnectionMultiplexer.GetSubscriber();
        }
        public void EntitySubcribe<T>(Action<string,DistributedEntityCommand<T>> callBack) where T : IEntity
        {
            _subscriberDb.Subscribe(typeof(T).FullName, (channel, value) =>
            {
                var cmd = JsonConvert.DeserializeObject<DistributedEntityCommand<T>>(value);
                callBack(channel, cmd);

                Console.WriteLine("\r\nDbCommand:done:" + channel + "\r\n" + value);
            });
        }

        public void EntityPublish<T>(DistributedEntityCommand<T> cmd) where T : IEntity
        {
            var redisValue = cmd.ToJson();
            var redisChannel = typeof (T).FullName;
            _subscriberDb.Publish(redisChannel, redisValue);

            Console.WriteLine("\r\nDbCommand:pushed:" + redisChannel+"\r\n"+ redisValue);
        }

        public void EntityDetailsPublish<T, TView>(DistributedEntityDetailsCommand<T, TView> cmd)
            where T : IEntity, new() where TView : AbstractDetails<T, TView>
        {
            var redisChannel = typeof(TView).FullName;
            var redisValue = cmd.ToJson();
            _subscriberDetails.Publish(redisChannel, redisValue);

            Console.WriteLine("\r\nFrontEndCommand:pushed:" + redisChannel + "\r\n" + redisValue);
        }

        public void EntityDetailsSubcribe<T, TView>(Action<string,DistributedEntityDetailsCommand<T, TView>> callBack )
            where T : IEntity, new() where TView : AbstractDetails<T, TView>
        {
            _subscriberDetails.Subscribe(typeof(TView).FullName, (channel, value) =>
            {
                var cmd = JsonConvert.DeserializeObject<DistributedEntityDetailsCommand<T,TView>>(value);
                callBack(channel, cmd);
                Console.WriteLine("\r\nFrontEndCommand:done:" + channel + "\r\n" + value);
            });
        }


        public void Publish<T>(DistributedCommand<T> cmd) where T : class
        {
            var redisChannel = typeof(T).FullName;
            var redisValue = cmd.ToJson();
            _subscriber.Publish(redisChannel, redisValue);

            Console.WriteLine("\r\nCommand:pushed:" + redisChannel + "\r\n" + redisValue);
        }

        public void Subscribe<T>(Action<string, DistributedCommand<T>> callBack) where T : class
        {
            _subscriber.Subscribe(typeof(T).FullName, (channel, value) =>
            {
                var cmd = JsonConvert.DeserializeObject<DistributedCommand<T>>(value);
                callBack(channel, cmd);

                Console.WriteLine("\r\nCommand:done:" + channel + "\r\n" + value);
            });
        }

    }
}