using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace JulJul.Core.Redis
{
    public static class RedisConnectionPool
    {
        private static Dictionary<RedisConfig, ConnectionMultiplexer> _pool;
        static IServer _server;
        static SocketManager _socketManager;
        static IConnectionMultiplexer _connectionMultiplexer;
        private static object locker = new object();
        private static RedisConfig _redisConfig;
        private static bool _isbooted;
        public static IConnectionMultiplexer CurrentConnectionMultiplexer
        {
            get { return _connectionMultiplexer.IsConnected ? _connectionMultiplexer : GetConnection(); }
        }

        public static IDatabase CurrentDatabase
        {
            get { return CurrentConnectionMultiplexer.GetDatabase(); }
        }

        public static IServer CurrentServer
        {
            get { return CurrentConnectionMultiplexer.GetServer(_redisConfig.Host, _redisConfig.Port); }
        }

        static RedisConnectionPool()
        {
            lock (locker)
            {
                _pool = new Dictionary<RedisConfig, ConnectionMultiplexer>();
                _socketManager = new SocketManager("JulJulCore", true);
                var configArray = ConfigurationManager.AppSettings["RedisConfig"].Split(';');
                _redisConfig = new RedisConfig
                {
                    Host = configArray[0],
                    Port = int.Parse(configArray[1]),
                    Pwd = configArray[2]
                };
            }
            
        }
        
        public static void Boot(RedisConfig configRedisInstance = null)
        {
            if (configRedisInstance != null)
            {
                lock (locker)
                {
                    _redisConfig = configRedisInstance;
                    _isbooted = true;
                }

                GetConnection(_redisConfig);
            }
        }

        public static ConnectionMultiplexer GetConnection(RedisConfig configRedisInstance = null)
        {
            if (!_isbooted)
            {
                throw new Exception("Must call RedisConnectionPool.Boot() at start application");
            }
            if (configRedisInstance != null)
            {
                _redisConfig = configRedisInstance;
            }

            var options = new ConfigurationOptions
            {
                EndPoints =
                {
                    {_redisConfig.Host, _redisConfig.Port}
                },
                Password = _redisConfig.Pwd,
                AllowAdmin = false,
                SyncTimeout = 5*1000,
                SocketManager = _socketManager,
                AbortOnConnectFail = false,
                ConnectTimeout = 5*1000,
            };

            lock (locker)
            {
                ConnectionMultiplexer c;
                if (_pool.TryGetValue(_redisConfig, out c) && c.IsConnected)
                {
                    return c;
                }

                if (c != null)
                {
                    c.Dispose();
                }

                c = ConnectionMultiplexer.Connect(options);

                _connectionMultiplexer = c;
                _pool[_redisConfig] = c;
                return c;
            }
        }
    }
}