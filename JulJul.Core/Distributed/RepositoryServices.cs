using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using JulJul.Core.Redis;
using StackExchange.Redis;

namespace JulJul.Core.Distributed
{
   public class RepositoryServices
    {
        static RepositoryServices _instance=new RepositoryServices();

        public static RepositoryServices Instance { get { return _instance; } }

        private ISubscriber _subscriber;

        public RepositoryServices()
        {
            RedisConnectionPool.Boot(new RedisConfig()
            {
                Host = "badpaybad.info",
                Port = 6379,
                Pwd = "badpaybad.info"
            });
            _subscriber = RedisConnectionPool.CurrentConnectionMultiplexer.GetSubscriber();

        }

        public void Subscribe<T>( Action<RepositoryCommand>  action) where T: class 
        {
            _subscriber.Subscribe(typeof(T).FullName, (channel, value) =>
            {
                action( new JavaScriptSerializer().Deserialize<RepositoryCommand>(value));
            });
        }

       public void PublishChange(RepositoryCommand cmd)
       {
            _subscriber.Publish(cmd.GetChannelKey(), cmd.ToJson());
        }
    }

    public enum CommandType
    {
        None,
        Add,
        Update,
        Delete
    }

    public class RepositoryCommand
    {
     public CommandType CommandType { get; set; }   
        public object Data { get; set; }
        public Type DataType { get { return Data.GetType(); } }

        public RepositoryCommand(CommandType cmdType, object data)
        {
            CommandType = cmdType;
            Data = data;
        }

        public string GetChannelKey()
        {
            return DataType.FullName;
        }

        public string ToJson()
        {
            return new JavaScriptSerializer().Serialize(this);
        }

        public T GetData<T>() where T : class
        {
            return (T) Data;
        }
    }
}
