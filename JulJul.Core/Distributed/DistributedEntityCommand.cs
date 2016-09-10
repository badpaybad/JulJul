using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JulJul.Core.Distributed
{
    public class DistributedEntityCommand<T> where T:IEntity
    {
        public DistributedDbCommandType CommandType { get; set; }
        public T Data { get; set; }

        public Type DataType
        {
            get
            {
                if (Data == null) return typeof (Nullable<>);
                return Data.GetType();
            }
        }

        public DistributedEntityCommand(T data, DistributedDbCommandType cmdType)
        {
            Data = data;
            CommandType = cmdType;
        } 

        public string ToJson()
        {
            if (Data == null) return string.Empty;
            return JsonConvert.SerializeObject(this);
        }

        public T GetData() 
        {
            if (Data == null) return default(T);

            return (T) Data;
        }

        public string GetChannelKey()
        {
            if (Data == null) throw new NoNullAllowedException();

            return DataType.FullName;
        }
    }
}