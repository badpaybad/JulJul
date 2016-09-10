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

    public class DistributedServices : IDistributedServices
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
        private IDatabase _dbQueue;
        private IDatabase _dbStack;

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
            _dbQueue = RedisConnectionPool.CurrentConnectionMultiplexer.GetDatabase();
            _dbStack = RedisConnectionPool.CurrentConnectionMultiplexer.GetDatabase();
        }

        public void EntityPublish<T>(DistributedEntityCommand<T> cmd) where T : IEntity
        {
            var redisChannel = typeof (T).FullName;
            if (!_subscriber.IsConnected(redisChannel))
            {
                var err = "Can not connect to pubsub services";
                Console.WriteLine(err);
                throw new Exception(err);
            }

            var redisValue = cmd.ToJson();

            switch (cmd.CommandBehavior)
            {
                case CommandBehavior.Queue:
                    _dbQueue.ListRightPush(cmd.CommandBehavior + redisChannel, redisValue);
                    //_dbQueue.Publish(redisChannel, redisValue);
                    _subscriber.Publish(redisChannel, redisValue);
                    break;
                case CommandBehavior.Stack:
                    _dbStack.ListRightPush(cmd.CommandBehavior + redisChannel, redisValue);
                    //_dbStack.Publish(redisChannel, redisValue);
                    _subscriber.Publish(redisChannel, redisValue);
                    break;
                default:
                    _subscriber.Publish(redisChannel, redisValue);
                    break;
            }
            Console.WriteLine("\r\nDbCommand:pushed:" + redisChannel + "\r\n" + redisValue);
        }

        public void EntitySubcribe<T>(Action<string, DistributedEntityCommand<T>> callBack) where T : IEntity
        {
            var redisChannel = typeof (T).FullName;
            if (!_subscriber.IsConnected(redisChannel))
            {
                var err = "Can not connect to pubsub services";
                Console.WriteLine(err);
                throw new Exception(err);
            }

            _subscriberDb.Subscribe(redisChannel, (channel, value) =>
            {
                var cmd = JsonConvert.DeserializeObject<DistributedEntityCommand<T>>(value);
                bool isDone = false;
                switch (cmd.CommandBehavior)
                {
                    case CommandBehavior.Queue:
                        var qv = _dbQueue.ListLeftPop(cmd.CommandBehavior + redisChannel);
                        if (qv.HasValue)
                        {
                            callBack(redisChannel,
                                JsonConvert.DeserializeObject<DistributedEntityCommand<T>>(qv));
                            isDone = true;
                        }
                        break;
                    case CommandBehavior.Stack:
                        var sv = _dbStack.ListRightPop(cmd.CommandBehavior + redisChannel);
                        if (sv.HasValue)
                        {
                            callBack(redisChannel,
                                JsonConvert.DeserializeObject<DistributedEntityCommand<T>>(sv));
                            isDone = true;
                        }
                        break;
                    default:
                        callBack(channel, cmd);
                        isDone = true;
                        break;
                }
                if(isDone)
                Console.WriteLine("\r\nDbCommand:done:" + channel + "\r\n" + value);
            });
        }

        public void EntityDetailsPublish<T, TView>(DistributedEntityDetailsCommand<T, TView> cmd)
            where T : IEntity, new() where TView : AbstractDetails<T, TView>
        {
            var redisChannel = typeof (TView).FullName;
            if (!_subscriber.IsConnected(redisChannel))
            {
                var err = "Can not connect to pubsub services";
                Console.WriteLine(err);
                throw new Exception(err);
            }

            var redisValue = cmd.ToJson();

            switch (cmd.CommandBehavior)
            {
                case CommandBehavior.Queue:
                    _dbQueue.ListRightPush(cmd.CommandBehavior + redisChannel, redisValue);
                    //_dbQueue.Publish(redisChannel, redisValue);
                    _subscriber.Publish(redisChannel, redisValue);
                    break;
                case CommandBehavior.Stack:
                    _dbStack.ListRightPush(cmd.CommandBehavior + redisChannel, redisValue);
                    //_dbStack.Publish(redisChannel, redisValue);
                    _subscriber.Publish(redisChannel, redisValue);
                    break;
                default:
                    _subscriber.Publish(redisChannel, redisValue);
                    break;
            }

            Console.WriteLine("\r\nFrontEndCommand:pushed:" + redisChannel + "\r\n" + redisValue);
        }

        public void EntityDetailsSubcribe<T, TView>(Action<string, DistributedEntityDetailsCommand<T, TView>> callBack)
            where T : IEntity, new() where TView : AbstractDetails<T, TView>
        {
            var redisChannel = typeof (TView).FullName;
            if (!_subscriber.IsConnected(redisChannel))
            {
                var err = "Can not connect to pubsub services";
                Console.WriteLine(err);
                throw new Exception(err);
            }

            _subscriberDetails.Subscribe(redisChannel, (channel, value) =>
            {
                var cmd = JsonConvert.DeserializeObject<DistributedEntityDetailsCommand<T, TView>>(value);
                bool isDone=false;
                switch (cmd.CommandBehavior)
                {
                    case CommandBehavior.Queue:
                        var qv = _dbQueue.ListLeftPop(cmd.CommandBehavior + redisChannel);
                        if (qv.HasValue)
                        {
                            callBack(redisChannel,
                                JsonConvert.DeserializeObject<DistributedEntityDetailsCommand<T, TView>>(qv));
                            isDone = true;
                        }
                        break;
                    case CommandBehavior.Stack:
                        var sv = _dbStack.ListRightPop(cmd.CommandBehavior + redisChannel);
                        if (sv.HasValue)
                        {
                            callBack(redisChannel,
                                JsonConvert.DeserializeObject<DistributedEntityDetailsCommand<T, TView>>(sv));
                            isDone = true;
                        }
                        break;
                    default:
                        callBack(channel, cmd);
                        isDone = true;
                        break;
                }
                if(isDone)
                Console.WriteLine("\r\nFrontEndCommand:done:" + channel + "\r\n" + value);
            });
        }

        public void Publish<T>(DistributedCommand<T> cmd) where T : class
        {
            var redisChannel = typeof (T).FullName;
            if (!_subscriber.IsConnected(redisChannel))
            {
                var err = "Can not connect to pubsub services";
                Console.WriteLine(err);
                throw new Exception(err);
            }
            var redisValue = cmd.ToJson();

            switch (cmd.CommandBehavior)
            {
                case CommandBehavior.Queue:
                    _dbQueue.ListRightPush(cmd.CommandBehavior + redisChannel, redisValue);
                    //_dbQueue.Publish(redisChannel, redisValue);
                    _subscriber.Publish(redisChannel, redisValue);
                    break;
                case CommandBehavior.Stack:
                    _dbStack.ListRightPush(cmd.CommandBehavior + redisChannel, redisValue);
                    //_dbStack.Publish(redisChannel, redisValue);
                    _subscriber.Publish(redisChannel, redisValue);
                    break;
                default:
                    _subscriber.Publish(redisChannel, redisValue);
                    break;
            }

            Console.WriteLine("\r\nCommand:pushed:" + redisChannel + "\r\n" + redisValue);
        }

        public void Subscribe<T>(Action<string, DistributedCommand<T>> callBack) where T : class
        {
            var redisChannel = typeof (T).FullName;
            if (!_subscriber.IsConnected(redisChannel))
            {
                var err = "Can not connect to pubsub services";
                Console.WriteLine(err);
                throw new Exception(err);
            }

            _subscriber.Subscribe(redisChannel, (channel, value) =>
            {
                var cmd = JsonConvert.DeserializeObject<DistributedCommand<T>>(value);
                bool isDone = false;
                switch (cmd.CommandBehavior)
                {
                    case CommandBehavior.Queue:
                        var qv = _dbQueue.ListLeftPop(cmd.CommandBehavior + redisChannel);
                        if (qv.HasValue)
                        {
                            callBack(redisChannel,
                                JsonConvert.DeserializeObject<DistributedCommand<T>>(qv));
                            isDone = true;
                        }
                        break;
                    case CommandBehavior.Stack:
                        var sv = _dbStack.ListRightPop(cmd.CommandBehavior + redisChannel);
                        if (sv.HasValue)
                        {
                            callBack(redisChannel,
                                JsonConvert.DeserializeObject<DistributedCommand<T>>(sv));
                            isDone = true;
                        }
                        break;
                    default:
                        callBack(channel, cmd);
                        isDone = true;
                        break;
                }
                if(isDone)
                Console.WriteLine("\r\nCommand:done:" + channel + "\r\n" + value);
            });
        }
    }
}